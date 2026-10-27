import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, Observable } from 'rxjs';
import { ValidityPeriodUtils } from '../../../core/utils/validity-period.utils';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { PermissionCodes } from '../../../core/permissions/constants/permission-codes';
import { PermissionService } from '../../../core/permissions/services/permission.service';
import { AuditHistoryComponent } from '../../../shared/audit-history/audit-history.component';
import {
 AdmissionTypeDefItem, AdmissionTypeFields, AdmissionTypeQuery, AdmissionTypeMutationResponse,
 AdmissionRequirementField
} from '../models/admission-types.model';
import { AdmissionTypesService } from '../services/admission-types.service';

type FormField = 'code' | 'name' | 'validFrom' | 'validTo';

/** Başvuru Grubu Tanımları: 14 tıklanabilir alan zorunluluğu ve mevcut CRUD deseni. */
@Component({
  selector: 'app-admission-type-list',
  standalone: true,
  imports: [DatePipe, ReactiveFormsModule, ButtonModule, CheckboxModule,
    ConfirmDialogModule, DialogModule, InputTextModule, TableModule, TagModule, AuditHistoryComponent],
  providers: [ConfirmationService],
  templateUrl: './admission-type-list.component.html',
  styleUrl: './admission-type-list.component.scss'
})
export class AdmissionTypeListComponent implements OnInit {
  private readonly service = inject(AdmissionTypesService);
  private readonly fb = inject(FormBuilder);
  private readonly permissions = inject(PermissionService);
  private readonly confirmation = inject(ConfirmationService);
  public readonly canCreate = computed(() => this.permissions.has(PermissionCodes.AdmissionTypes.Create));
  public readonly canUpdate = computed(() => this.permissions.has(PermissionCodes.AdmissionTypes.Update));
  public readonly canDelete = computed(() => this.permissions.has(PermissionCodes.AdmissionTypes.Delete));
  public readonly canExport = computed(() => this.permissions.has(PermissionCodes.AdmissionTypes.Query));
  public readonly canAudit = computed(() => this.permissions.has(PermissionCodes.AuditLogs.View));
  public readonly records = signal<AdmissionTypeDefItem[]>([]);
  public readonly totalCount = signal(0);
  public readonly pageNumber = signal(1);
  public readonly pageSize = signal(20);
  public readonly loading = signal(false);
  public readonly exporting = signal(false);
  public readonly saving = signal(false);
  public readonly actionLoadingId = signal<string | null>(null);
  public readonly successMessage = signal<string | null>(null);
  public readonly errorMessage = signal<string | null>(null);
  public readonly dialogVisible = signal(false);
  public readonly editing = signal<AdmissionTypeDefItem | null>(null);
  public readonly formError = signal<string | null>(null);
  public readonly fieldServerErrors = signal<Record<string,string>>({});
  public readonly attemptedSave = signal(false);
  public readonly filtersVisible = signal(false);
  public readonly auditRecord = signal<AdmissionTypeDefItem | null>(null);
  private listRequestId = 0;
  private dialogRequestId = 0;

  public readonly filterForm = this.fb.nonNullable.group({ search: '', includeInactive: false });
  public readonly columnFilterForm = this.fb.group({
    isActive: this.fb.control<boolean | null>(null),
  });
  /** 14 bağımsız boolean kontrol, her biri tıklandığında doğrudan formda güncellenir. */
  public readonly itemForm = this.fb.nonNullable.group({
    code: [0, [Validators.required, Validators.min(1), Validators.max(2147483647), Validators.pattern(/^[0-9]+$/)]],
    name: ['', [Validators.required, Validators.maxLength(250)]],
    forcesCommandRequired: [false],
    rankRequired: [false],
    militaryClassRequired: [false],
    militaryUnitRequired: [false],
    senderChairRequired: [false],
    militaryOfficeRequired: [false],
    registryNumberRequired: [false],
    militarySchoolRequired: [false],
    lastNominationDateRequired: [false],
    heightWeightRequired: [false],
    nthAdmissionRequired: [false],
    dispatchDocumentDateRequired: [false],
    dispatchDocumentNoRequired: [false],
    pertemApprovalRequired: [false],
    validFrom: [''],
    validTo: ['']
  });

  /** Yalnız ekran sunumu için; API'ye ekstra grup veya alan gönderilmez. */
  public readonly requirementGroups: readonly {
    title: string; fields: readonly { key: AdmissionRequirementField; label: string }[]
  }[] = [
    { title: 'Askerî Bilgiler', fields: [
      { key: 'forcesCommandRequired', label: 'Kuvvet Komutanlığı' },
      { key: 'rankRequired', label: 'Rütbe' },
      { key: 'militaryClassRequired', label: 'Askerî Sınıf' },
      { key: 'militaryUnitRequired', label: 'Birlik' },
      { key: 'senderChairRequired', label: 'Gönderen Makam' },
      { key: 'militaryOfficeRequired', label: 'Askerlik Şubesi' },
      { key: 'registryNumberRequired', label: 'Sicil Numarası' },
      { key: 'militarySchoolRequired', label: 'Askerî Okul' },
    ] },
    { title: 'Başvuru ve Sağlık', fields: [
      { key: 'lastNominationDateRequired', label: 'Son Adaylık Tarihi' },
      { key: 'heightWeightRequired', label: 'Boy ve Kilo' },
      { key: 'nthAdmissionRequired', label: 'Kaçıncı İşlem Gerekir' },
    ] },
    { title: 'Belgeler ve Onay', fields: [
      { key: 'dispatchDocumentDateRequired', label: 'Sevk Belgesi Tarihi' },
      { key: 'dispatchDocumentNoRequired', label: 'Sevk Belgesi Numarası' },
      { key: 'pertemApprovalRequired', label: 'PERTEM Onayı' },
    ] },
  ];

  /** Sayfada seçim yoğunluğunun okunabilmesi için hesaplanan, DB dışı değer. */
  public requiredCount(item: AdmissionTypeDefItem): number {
    return this.requirementGroups.reduce((sum, group) =>
      sum + group.fields.filter(field => item[field.key]).length, 0);
  }
  public readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));
  public readonly rangeStart = computed(() => this.totalCount() ? (this.pageNumber()-1)*this.pageSize()+1 : 0);
  public readonly rangeEnd = computed(() => Math.min(this.totalCount(), this.pageNumber()*this.pageSize()));

  public constructor() {
    effect(onCleanup => {
      if (!this.successMessage()) return;
      const timer = setTimeout(() => this.successMessage.set(null), 8000);
      onCleanup(() => clearTimeout(timer));
    });
  }
  public ngOnInit(): void {
    this.loadRecords();
  }

  private query(): AdmissionTypeQuery {
    const filters = this.columnFilterForm.getRawValue();
    const search = this.filterForm.getRawValue();
    return {
      search: search.search,
      includeInactive: search.includeInactive || filters.isActive === false,
      isActive: filters.isActive,
      pageNumber: this.pageNumber(), pageSize: this.pageSize()
    };
  }
  public loadRecords(): void {
    const requestId = ++this.listRequestId;
    this.loading.set(true);
    this.errorMessage.set(null);
    this.service.list(this.query()).pipe(finalize(() => {
      if (requestId === this.listRequestId) this.loading.set(false);
    })).subscribe({
      next: result => {
        if (requestId !== this.listRequestId) return;
        this.records.set(result.items);
        this.totalCount.set(result.totalCount);
      },
      error: (err: HttpErrorResponse) => {
        if (requestId !== this.listRequestId) return;
        this.records.set([]);
        this.totalCount.set(0);
        this.errorMessage.set(this.errorText(err));
      }
    });
  }
  public search(): void { this.pageNumber.set(1); this.loadRecords(); }
  public applyFilters(): void { this.pageNumber.set(1); this.loadRecords(); }
  public clearFilters(): void {
    this.filterForm.reset({search:'', includeInactive:false});
    this.columnFilterForm.reset({ isActive:null });
    this.pageNumber.set(1);
    this.loadRecords();
  }
  public goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.pageNumber() || this.loading()) return;
    this.pageNumber.set(page);
    this.loadRecords();
  }
  public onPageSizeChange(event: Event): void {
    const size = Number((event.target as HTMLSelectElement).value);
    if (![10,20,50,100].includes(size) || size===this.pageSize()) return;
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.loadRecords();
  }
  /** Yalnızca Query izniyle filtrelenmiş grupları Excel indirir. */
  public exportExcel(): void {
    if (!this.canExport() || this.exporting()) return;
    this.exporting.set(true);
    this.service.export(this.query()).pipe(finalize(() => this.exporting.set(false))).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Basvuru_Grubu_Tanimlari_' + new Date().toISOString().slice(0,10) + '.xlsx';
        a.click();
        setTimeout(() => URL.revokeObjectURL(url), 1000);
        this.successMessage.set('Başvuru Grubu listesi Excel olarak indirildi.');
      },
      error: (err: HttpErrorResponse) => this.errorMessage.set(this.errorText(err))
    });
  }
  /** Formu yeni kayıt için temizler; geçmiş kaydın alan hataları kalmaz. */
  public openCreateDialog(): void {
    if (!this.canCreate() || this.saving()) return;
    ++this.dialogRequestId;
    this.editing.set(null);
    this.itemForm.reset({ code:0, name:'', forcesCommandRequired:false, rankRequired:false, militaryClassRequired:false, militaryUnitRequired:false, senderChairRequired:false, militaryOfficeRequired:false, registryNumberRequired:false, militarySchoolRequired:false, lastNominationDateRequired:false, heightWeightRequired:false, nthAdmissionRequired:false, dispatchDocumentDateRequired:false, dispatchDocumentNoRequired:false, pertemApprovalRequired:false, validFrom:'', validTo:'' });
    this.attemptedSave.set(false);
    this.fieldServerErrors.set({});
    this.formError.set(null);
    this.dialogVisible.set(true);
  }
  /** Güncel xmin sürümüyle düzenleme diyaloğunu açar. */
  public openEditDialog(item: AdmissionTypeDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId() || this.saving()) return;
    const requestId = ++this.dialogRequestId;
    this.actionLoadingId.set(item.id);
    this.service.getById(item.id).pipe(finalize(() => {
      if (requestId === this.dialogRequestId) this.actionLoadingId.set(null);
    })).subscribe({
      next: detail => {
        if (requestId !== this.dialogRequestId) return;
        this.editing.set(detail);
        this.itemForm.reset({ code: detail.code, name: detail.name,
          forcesCommandRequired: detail.forcesCommandRequired,
          rankRequired: detail.rankRequired,
          militaryClassRequired: detail.militaryClassRequired,
          militaryUnitRequired: detail.militaryUnitRequired,
          senderChairRequired: detail.senderChairRequired,
          militaryOfficeRequired: detail.militaryOfficeRequired,
          registryNumberRequired: detail.registryNumberRequired,
          militarySchoolRequired: detail.militarySchoolRequired,
          lastNominationDateRequired: detail.lastNominationDateRequired,
          heightWeightRequired: detail.heightWeightRequired,
          nthAdmissionRequired: detail.nthAdmissionRequired,
          dispatchDocumentDateRequired: detail.dispatchDocumentDateRequired,
          dispatchDocumentNoRequired: detail.dispatchDocumentNoRequired,
          pertemApprovalRequired: detail.pertemApprovalRequired,
          validFrom: ValidityPeriodUtils.toLocalInput(detail.validFrom),
          validTo: ValidityPeriodUtils.toLocalInput(detail.validTo) });
        this.attemptedSave.set(false);
        this.fieldServerErrors.set({});
        this.formError.set(null);
        this.dialogVisible.set(true);
      },
      error: (err: HttpErrorResponse) => {
        if (requestId === this.dialogRequestId) this.errorMessage.set(this.errorText(err));
      }
    });
  }
  public onDialogVisibleChange(visible: boolean): void {
    if (!visible && this.saving()) return;
    this.dialogVisible.set(visible);
    if (!visible) ++this.dialogRequestId;
  }
  public fieldError(field: FormField): string | null {
    if (this.fieldServerErrors()[field]) return this.fieldServerErrors()[field];
    const control = this.itemForm.controls[field];
    if (!this.attemptedSave() && !control.touched) return null;
    if (control.hasError('required')) return 'Bu alan zorunludur.';
    if (control.hasError('maxlength')) return 'Karakter sınırı aşıldı.';
    if (control.hasError('pattern') || control.hasError('min') || control.hasError('max'))
      return 'Kod 1 ile 2147483647 arasında tam sayı olmalıdır.';
    if (field === 'validTo') {
      const v=this.itemForm.getRawValue();
      if (!ValidityPeriodUtils.hasValidRange(v.validFrom, v.validTo))
        return 'Bitiş tarihi başlangıç tarihinden sonra olmalıdır.';
    }
    return null;
  }
  private focusField(field: string): void {
    requestAnimationFrame(() => {
      if (!this.dialogVisible()) return;
      const element = document.getElementById('admission-types-' + field);
      element?.scrollIntoView({ behavior:'smooth', block:'center' });
      element?.focus({ preventScroll:true });
    });
  }
  /** Sınıf alanlarını kaydeder; aktiflik ayrı status işlemiyle değişir. */
  public save(): void {
    if (this.saving() || (this.editing() ? !this.canUpdate() : !this.canCreate())) return;
    this.attemptedSave.set(true);
    this.formError.set(null);
    this.fieldServerErrors.set({});
    if (this.itemForm.invalid) {
      this.itemForm.markAllAsTouched();
      const first = (['code','name'] as FormField[])
        .find(f => this.itemForm.controls[f].invalid);
      if (first) this.focusField(first);
      return;
    }
    const values = this.itemForm.getRawValue();
    if (!ValidityPeriodUtils.hasValidRange(values.validFrom, values.validTo)) {
      this.focusField('validTo');
      return;
    }
    const fields: AdmissionTypeFields = {
      code: values.code,
      name: values.name.trim(),
      forcesCommandRequired: values.forcesCommandRequired,
      rankRequired: values.rankRequired,
      militaryClassRequired: values.militaryClassRequired,
      militaryUnitRequired: values.militaryUnitRequired,
      senderChairRequired: values.senderChairRequired,
      militaryOfficeRequired: values.militaryOfficeRequired,
      registryNumberRequired: values.registryNumberRequired,
      militarySchoolRequired: values.militarySchoolRequired,
      lastNominationDateRequired: values.lastNominationDateRequired,
      heightWeightRequired: values.heightWeightRequired,
      nthAdmissionRequired: values.nthAdmissionRequired,
      dispatchDocumentDateRequired: values.dispatchDocumentDateRequired,
      dispatchDocumentNoRequired: values.dispatchDocumentNoRequired,
      pertemApprovalRequired: values.pertemApprovalRequired,
      ...ValidityPeriodUtils.fromLocalInputs(values.validFrom, values.validTo)
    };
    const edited = this.editing();
    const request: Observable<AdmissionTypeMutationResponse> = edited
      ? this.service.update({ ...fields, id: edited.id, version: edited.version })
      : this.service.create(fields);
    this.saving.set(true);
    request.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.dialogVisible.set(false);
        ++this.dialogRequestId;
        this.editing.set(null);
        this.successMessage.set(edited ? 'Başvuru Grubu güncellendi.' : 'Yeni grup oluşturuldu.');
        this.loadRecords();
      },
      error: (err: HttpErrorResponse) => this.handleSaveError(err)
    });
  }
  private handleSaveError(err: HttpErrorResponse): void {
    const errors: unknown = err.error?.errors;
    const mapped: Record<string,string> = {};
    if (errors && typeof errors === 'object' && !Array.isArray(errors)) {
      for (const [key, value] of Object.entries(errors)) {
        const name = key.charAt(0).toLowerCase() + key.slice(1);
        if (Array.isArray(value) && typeof value[0] === 'string') mapped[name] = value[0];
      }
    }
    this.fieldServerErrors.set(mapped);
    const first = (Object.keys(this.itemForm.controls) as FormField[]).find(key => mapped[key]);
    if (first) {
      this.formError.set('İşaretlenen alanları kontrol ederek tekrar kaydediniz.');
      this.focusField(first);
    } else this.formError.set(this.errorText(err));
  }
  public confirmStatusChange(item: AdmissionTypeDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId()) return;
    this.confirmation.confirm({
      header: 'Başvuru Grubu Durumu',
      message: item.name + ' kaydını ' + (item.isActive ? 'pasifleştirmek' : 'aktifleştirmek') + ' istiyor musunuz?',
      acceptLabel: 'Evet', rejectLabel:'Vazgeç',
      accept: () => this.runRowAction(item,
        this.service.changeStatus({ id:item.id, version:item.version, isActive:!item.isActive }),
        'Başvuru Grubu durumu güncellendi.')
    });
  }
  public confirmDelete(item: AdmissionTypeDefItem): void {
    if (!this.canDelete() || this.actionLoadingId()) return;
    this.confirmation.confirm({
      header: 'Başvuru Grubu Sil', message: item.name + ' kaydı silinsin mi?',
      acceptLabel:'Sil', rejectLabel:'Vazgeç', acceptButtonStyleClass:'p-button-danger',
      accept: () => this.runRowAction(item,
        this.service.delete(item.id, item.version), 'Başvuru Grubu kaydı silindi.')
    });
  }
  private runRowAction(item: AdmissionTypeDefItem, request: Observable<unknown>, message: string): void {
    if (this.actionLoadingId()) return;
    this.actionLoadingId.set(item.id);
    request.pipe(finalize(() => this.actionLoadingId.set(null))).subscribe({
      next: () => { this.successMessage.set(message); this.loadRecords(); },
      error: (err: HttpErrorResponse) => this.errorMessage.set(this.errorText(err))
    });
  }
  public openAudit(item: AdmissionTypeDefItem): void { if (this.canAudit()) this.auditRecord.set(item); }
  private errorText(err: HttpErrorResponse): string {
    if (typeof err.error?.detail === 'string' && err.error.detail.trim()) return err.error.detail;
    if (err.status === 403) return 'Bu işlem için yetkiniz yok.';
    if (err.status === 404) return 'Başvuru Grubu kaydı bulunamadı.';
    if (err.status === 409) return 'Kayıt değişmiş veya bağlı kayıtları var. Listeyi yenileyiniz.';
    return 'İşlem tamamlanamadı, tekrar deneyiniz.';
  }
}
