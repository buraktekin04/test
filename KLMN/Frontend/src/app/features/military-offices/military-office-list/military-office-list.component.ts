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
 MilitaryOfficeDefItem, MilitaryOfficeFields, MilitaryOfficeQuery, MilitaryOfficeDefMutationResponse, MilitaryOfficeOption
} from '../models/military-offices.model';
import { MilitaryOfficeService } from '../services/military-offices.service';

type FormField = 'code' | 'name' | 'externalCode' | 'cityId' | 'validFrom' | 'validTo';

/** Askerlik Şubesi Tanımları: backend filtreleri, ayrı işlem yetkileri ve sade form. */
@Component({
  selector: 'app-military-office-list',
  standalone: true,
  imports: [DatePipe, ReactiveFormsModule, ButtonModule, CheckboxModule,
    ConfirmDialogModule, DialogModule, InputTextModule, TableModule, TagModule, AuditHistoryComponent],
  providers: [ConfirmationService],
  templateUrl: './military-office-list.component.html',
  styleUrl: './military-office-list.component.scss'
})
export class MilitaryOfficeListComponent implements OnInit {
  private readonly service = inject(MilitaryOfficeService);
  private readonly fb = inject(FormBuilder);
  private readonly permissions = inject(PermissionService);
  private readonly confirmation = inject(ConfirmationService);
  public readonly canCreate = computed(() => this.permissions.has(PermissionCodes.MilitaryOffices.Create));
  public readonly canUpdate = computed(() => this.permissions.has(PermissionCodes.MilitaryOffices.Update));
  public readonly canDelete = computed(() => this.permissions.has(PermissionCodes.MilitaryOffices.Delete));
  public readonly canExport = computed(() => this.permissions.has(PermissionCodes.MilitaryOffices.Query));
  public readonly canAudit = computed(() => this.permissions.has(PermissionCodes.AuditLogs.View));
  public readonly records = signal<MilitaryOfficeDefItem[]>([]);
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
  public readonly editing = signal<MilitaryOfficeDefItem | null>(null);
  public readonly formError = signal<string | null>(null);
  public readonly fieldServerErrors = signal<Record<string,string>>({});
  public readonly attemptedSave = signal(false);
  public readonly filtersVisible = signal(false);
  public readonly auditRecord = signal<MilitaryOfficeDefItem | null>(null);
  /** Şehir seçenekleri yalnızca Askerlik Şubesi yetkisiyle alınır. */
  public readonly cityOptions = signal<MilitaryOfficeOption[]>([]);
  public readonly optionsError = signal<string | null>(null);
  public readonly optionsLoading = signal(false);
  private listRequestId = 0;
  private dialogRequestId = 0;
  private optionsRequestId = 0;

  public readonly filterForm = this.fb.nonNullable.group({ search: '', includeInactive: false });
  public readonly columnFilterForm = this.fb.group({
    isActive: this.fb.control<boolean | null>(null),
    cityId: ''
  });
  public readonly itemForm = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.maxLength(100), Validators.pattern(/^[A-Za-z0-9_-]+$/)]],
    name: ['', [Validators.required, Validators.maxLength(250)]],
    externalCode: ['', [Validators.maxLength(100)]],
    cityId: ['', Validators.required],
    validFrom: [''],
    validTo: ['']
  });
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
    this.loadOptions();
  }

  /** Şube izniyle şehir seçeneklerini yükler; düzenlemede önceki pasif seçim korunur. */
  public loadOptions(selectedCityId?: string | null): void {
    // Ekran açılışındaki eski istek, düzenlenen kaydın pasif seçimlerini ezmemeli.
    const requestId = ++this.optionsRequestId;
    this.optionsLoading.set(true);
    this.optionsError.set(null);
    this.service.options(selectedCityId)
      .pipe(finalize(() => {
        if (requestId === this.optionsRequestId) this.optionsLoading.set(false);
      })).subscribe({
        next: result => {
          if (requestId !== this.optionsRequestId) return;
          this.cityOptions.set(result);
        },
        error: (err: HttpErrorResponse) => {
          if (requestId !== this.optionsRequestId) return;
          this.cityOptions.set([]);
          this.optionsError.set(this.errorText(err));
        }
      });
  }

  private query(): MilitaryOfficeQuery {
    const filters = this.columnFilterForm.getRawValue();
    const search = this.filterForm.getRawValue();
    return {
      search: search.search,
      includeInactive: search.includeInactive || filters.isActive === false,
      isActive: filters.isActive,
      cityId: filters.cityId || null,
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
    this.columnFilterForm.reset({ isActive:null, cityId:'' });
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
  public exportExcel(): void {
    if (!this.canExport() || this.exporting()) return;
    this.exporting.set(true);
    this.service.export(this.query()).pipe(finalize(() => this.exporting.set(false))).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Askerlik_Subesi_Tanimlari_' + new Date().toISOString().slice(0,10) + '.xlsx';
        a.click();
        setTimeout(() => URL.revokeObjectURL(url), 1000);
        this.successMessage.set('Askerlik Şubesi listesi Excel olarak indirildi.');
      },
      error: (err: HttpErrorResponse) => this.errorMessage.set(this.errorText(err))
    });
  }
  public openCreateDialog(): void {
    if (!this.canCreate() || this.saving()) return;
    ++this.dialogRequestId;
    this.editing.set(null);
    this.itemForm.reset({ code:'', name:'', externalCode:'', cityId:'', validFrom:'', validTo:'' });
    this.attemptedSave.set(false);
    this.fieldServerErrors.set({});
    this.formError.set(null);
    this.dialogVisible.set(true);
  }
  public openEditDialog(item: MilitaryOfficeDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId() || this.saving()) return;
    const requestId = ++this.dialogRequestId;
    this.actionLoadingId.set(item.id);
    this.service.getById(item.id).pipe(finalize(() => {
      if (requestId === this.dialogRequestId) this.actionLoadingId.set(null);
    })).subscribe({
      next: detail => {
        if (requestId !== this.dialogRequestId) return;
        this.editing.set(detail);
        this.loadOptions(detail.cityId);
        this.itemForm.reset({ code: detail.code, name: detail.name,
          externalCode: detail.externalCode ?? '',
          cityId: detail.cityId ?? '',
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
    if (control.hasError('pattern')) return 'Girdiğiniz değer uygun biçimde değil.';
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
      const element = document.getElementById('military-offices-' + field);
      element?.scrollIntoView({ behavior:'smooth', block:'center' });
      element?.focus({ preventScroll:true });
    });
  }
  public save(): void {
    if (this.saving() || (this.editing() ? !this.canUpdate() : !this.canCreate())) return;
    this.attemptedSave.set(true);
    this.formError.set(null);
    this.fieldServerErrors.set({});
    if (this.itemForm.invalid) {
      this.itemForm.markAllAsTouched();
      const first = (['code','name','cityId'] as FormField[])
        .find(f => this.itemForm.controls[f].invalid);
      if (first) this.focusField(first);
      return;
    }
    const values = this.itemForm.getRawValue();
    if (!ValidityPeriodUtils.hasValidRange(values.validFrom, values.validTo)) {
      this.focusField('validTo');
      return;
    }
    if (this.optionsError()) {
      this.formError.set('Şehir seçenekleri yüklenemedi; tekrar deneyiniz.');
      return;
    }
    const fields: MilitaryOfficeFields = {
      code: values.code.trim().toUpperCase(),
      name: values.name.trim(),
      externalCode: values.externalCode.trim() || null,
      cityId: values.cityId || null,
      ...ValidityPeriodUtils.fromLocalInputs(values.validFrom, values.validTo)
    };
    const edited = this.editing();
    const request: Observable<MilitaryOfficeDefMutationResponse> = edited
      ? this.service.update({ ...fields, id: edited.id, version: edited.version })
      : this.service.create(fields);
    this.saving.set(true);
    request.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.dialogVisible.set(false);
        ++this.dialogRequestId;
        this.editing.set(null);
        this.successMessage.set(edited ? 'Askerlik Şubesi güncellendi.' : 'Yeni şube oluşturuldu.');
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
  public confirmStatusChange(item: MilitaryOfficeDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId()) return;
    this.confirmation.confirm({
      header: 'Askerlik Şubesi Durumu',
      message: item.name + ' kaydını ' + (item.isActive ? 'pasifleştirmek' : 'aktifleştirmek') + ' istiyor musunuz?',
      acceptLabel: 'Evet', rejectLabel:'Vazgeç',
      accept: () => this.runRowAction(item,
        this.service.changeStatus({ id:item.id, version:item.version, isActive:!item.isActive }),
        'Askerlik Şubesi durumu güncellendi.')
    });
  }
  public confirmDelete(item: MilitaryOfficeDefItem): void {
    if (!this.canDelete() || this.actionLoadingId()) return;
    this.confirmation.confirm({
      header: 'Askerlik Şubesi Sil', message: item.name + ' kaydı silinsin mi?',
      acceptLabel:'Sil', rejectLabel:'Vazgeç', acceptButtonStyleClass:'p-button-danger',
      accept: () => this.runRowAction(item,
        this.service.delete(item.id, item.version), 'Askerlik Şubesi kaydı silindi.')
    });
  }
  private runRowAction(item: MilitaryOfficeDefItem, request: Observable<unknown>, message: string): void {
    if (this.actionLoadingId()) return;
    this.actionLoadingId.set(item.id);
    request.pipe(finalize(() => this.actionLoadingId.set(null))).subscribe({
      next: () => { this.successMessage.set(message); this.loadRecords(); },
      error: (err: HttpErrorResponse) => this.errorMessage.set(this.errorText(err))
    });
  }
  public openAudit(item: MilitaryOfficeDefItem): void { if (this.canAudit()) this.auditRecord.set(item); }
  private errorText(err: HttpErrorResponse): string {
    if (typeof err.error?.detail === 'string' && err.error.detail.trim()) return err.error.detail;
    if (err.status === 403) return 'Bu işlem için yetkiniz yok.';
    if (err.status === 404) return 'Askerlik Şubesi kaydı bulunamadı.';
    if (err.status === 409) return 'Kayıt değişmiş veya bağlı kayıtları var. Listeyi yenileyiniz.';
    return 'İşlem tamamlanamadı, tekrar deneyiniz.';
  }
}
