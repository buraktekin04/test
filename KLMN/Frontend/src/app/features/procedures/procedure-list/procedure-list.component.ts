import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
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
 ProcedureDefItem, ProcedureFields, ProcedureQuery, ProcedureDefMutationResponse, ProcedureTypeOption
} from '../models/procedures.model';
import { ProcedureService } from '../services/procedures.service';

type FormField = 'procedureCode' | 'name' | 'validFrom' | 'validTo' | 'procedureType';

/** Tetkik Tanımları: backend filtreleri, ayrı işlem yetkileri ve sade form. */
@Component({
  selector: 'app-procedure-list',
  standalone: true,
  imports: [DatePipe, RouterLink, ReactiveFormsModule, ButtonModule, CheckboxModule,
    ConfirmDialogModule, DialogModule, InputTextModule, TableModule, TagModule, AuditHistoryComponent],
  providers: [ConfirmationService],
  templateUrl: './procedure-list.component.html',
  styleUrl: './procedure-list.component.scss'
})
export class ProcedureListComponent implements OnInit {
  private readonly service = inject(ProcedureService);
  private readonly fb = inject(FormBuilder);
  private readonly permissions = inject(PermissionService);
  private readonly confirmation = inject(ConfirmationService);
  public readonly canCreate = computed(() => this.permissions.has(PermissionCodes.Procedures.Create));
  public readonly canUpdate = computed(() => this.permissions.has(PermissionCodes.Procedures.Update));
  public readonly canDelete = computed(() => this.permissions.has(PermissionCodes.Procedures.Delete));
  public readonly canExport = computed(() => this.permissions.has(PermissionCodes.Procedures.Query));
  public readonly canAudit = computed(() => this.permissions.has(PermissionCodes.AuditLogs.View));
  public readonly canOpenEquivalents = computed(() => this.permissions.hasModule('EquivalentProcedures'));
  public readonly typeOptions = signal<ProcedureTypeOption[]>([]);
  public readonly records = signal<ProcedureDefItem[]>([]);
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
  public readonly editing = signal<ProcedureDefItem | null>(null);
  public readonly formError = signal<string | null>(null);
  public readonly fieldServerErrors = signal<Record<string,string>>({});
  public readonly attemptedSave = signal(false);
  public readonly filtersVisible = signal(false);
  public readonly auditRecord = signal<ProcedureDefItem | null>(null);
  private listRequestId = 0;
  private dialogRequestId = 0;

  public readonly filterForm = this.fb.nonNullable.group({ search: '', includeInactive: false });
  public readonly columnFilterForm = this.fb.group({
    isActive: this.fb.control<boolean | null>(null),
  });
  public readonly itemForm = this.fb.nonNullable.group({
    procedureCode: ['', [Validators.required, Validators.maxLength(100), Validators.pattern(/^[A-Za-z0-9_-]+$/)]],
    name: ['', [Validators.required, Validators.maxLength(250)]],
    validFrom: [''],
    validTo: [''],
    requiresNumericResult:false,
    requiresReferenceRange:false,
    requiresDeviationInfo:false,
    procedureType: this.fb.control<number|null>(null,Validators.required)
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
    this.loadTypeOptions();
  }

  /** Diğer modülün permission'ına ihtiyaç duymadan dinamik türleri getirir. */
  private loadTypeOptions(selectedCode?:number|null):void {
    this.service.typeOptions(selectedCode).subscribe({
      next:types=>this.typeOptions.set(types),
      error:(e:HttpErrorResponse)=>this.errorMessage.set(this.errorText(e))
    });
  }

  private query(): ProcedureQuery {
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
  /** Yalnızca Query izniyle filtrelenmiş kararları Excel indirir. */
  public exportExcel(): void {
    if (!this.canExport() || this.exporting()) return;
    this.exporting.set(true);
    this.service.export(this.query()).pipe(finalize(() => this.exporting.set(false))).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Tetkik_Tanimlari_' + new Date().toISOString().slice(0,10) + '.xlsx';
        a.click();
        setTimeout(() => URL.revokeObjectURL(url), 1000);
        this.successMessage.set('Tetkik listesi Excel olarak indirildi.');
      },
      error: (err: HttpErrorResponse) => this.errorMessage.set(this.errorText(err))
    });
  }
  /** Formu yeni kayıt için temizler; geçmiş kaydın alan hataları kalmaz. */
  public openCreateDialog(): void {
    if (!this.canCreate() || this.saving()) return;
    ++this.dialogRequestId;
    this.editing.set(null);
    this.itemForm.reset({procedureCode:'',name:'',validFrom:'',validTo:'',
      requiresNumericResult:false,requiresReferenceRange:false,requiresDeviationInfo:false,procedureType:null});
    this.loadTypeOptions();
    this.attemptedSave.set(false);
    this.fieldServerErrors.set({});
    this.formError.set(null);
    this.dialogVisible.set(true);
  }
  /** Güncel xmin sürümüyle düzenleme diyaloğunu açar. */
  public openEditDialog(item: ProcedureDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId() || this.saving()) return;
    const requestId = ++this.dialogRequestId;
    this.actionLoadingId.set(item.id);
    this.service.getById(item.id).pipe(finalize(() => {
      if (requestId === this.dialogRequestId) this.actionLoadingId.set(null);
    })).subscribe({
      next: detail => {
        if (requestId !== this.dialogRequestId) return;
        this.editing.set(detail);
        this.itemForm.reset({ procedureCode: detail.procedureCode, name: detail.name,
          validFrom: ValidityPeriodUtils.toLocalInput(detail.validFrom),
          validTo: ValidityPeriodUtils.toLocalInput(detail.validTo),
          requiresNumericResult:detail.requiresNumericResult,
          requiresReferenceRange:detail.requiresReferenceRange,
          requiresDeviationInfo:detail.requiresDeviationInfo,
          procedureType:detail.procedureType });
        this.loadTypeOptions(detail.procedureType);
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
      const element = document.getElementById('procedures-' + field);
      element?.scrollIntoView({ behavior:'smooth', block:'center' });
      element?.focus({ preventScroll:true });
    });
  }
  /** Tetkik alanlarını kaydeder; aktiflik ayrı status işlemiyle değişir. */
  public save(): void {
    if (this.saving() || (this.editing() ? !this.canUpdate() : !this.canCreate())) return;
    this.attemptedSave.set(true);
    this.formError.set(null);
    this.fieldServerErrors.set({});
    if (this.itemForm.invalid) {
      this.itemForm.markAllAsTouched();
      const first = (['procedureCode','name','procedureType'] as FormField[])
        .find(f => this.itemForm.controls[f].invalid);
      if (first) this.focusField(first);
      return;
    }
    const values = this.itemForm.getRawValue();
    if (!ValidityPeriodUtils.hasValidRange(values.validFrom, values.validTo)) {
      this.focusField('validTo');
      return;
    }
    const fields: ProcedureFields = {
      procedureCode: values.procedureCode.trim().toUpperCase(),
      name: values.name.trim(),
      requiresNumericResult:values.requiresNumericResult,
      requiresReferenceRange:values.requiresReferenceRange,
      requiresDeviationInfo:values.requiresDeviationInfo,
      validFrom: ValidityPeriodUtils.toUtcIso(values.validFrom),
      validTo: ValidityPeriodUtils.toUtcIso(values.validTo),
      procedureType: values.procedureType!
    };
    const edited = this.editing();
    const request: Observable<ProcedureDefMutationResponse> = edited
      ? this.service.update({ ...fields, id: edited.id, version: edited.version })
      : this.service.create(fields);
    this.saving.set(true);
    request.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.dialogVisible.set(false);
        ++this.dialogRequestId;
        this.editing.set(null);
        this.successMessage.set(edited ? 'Tetkik güncellendi.' : 'Yeni tetkik oluşturuldu.');
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
      this.formError.set('Belirtilen alanları kontrol ederek tekrar kaydediniz.');
      this.focusField(first);
    } else this.formError.set(this.errorText(err));
  }
  public confirmStatusChange(item: ProcedureDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId()) return;
    this.confirmation.confirm({
      header: 'Tetkik Durumu',
      message: item.name + ' kaydını ' + (item.isActive ? 'pasifleştirmek' : 'aktifleştirmek') + ' istiyor musunuz?',
      acceptLabel: 'Evet', rejectLabel:'Vazgeç',
      accept: () => this.runRowAction(item,
        this.service.changeStatus({ id:item.id, version:item.version, isActive:!item.isActive }),
        'Tetkik durumu güncellendi.')
    });
  }
  public confirmDelete(item: ProcedureDefItem): void {
    if (!this.canDelete() || this.actionLoadingId()) return;
    this.confirmation.confirm({
      header: 'Tetkik Sil', message: item.name + ' kaydı silinsin mi?',
      acceptLabel:'Sil', rejectLabel:'Vazgeç', acceptButtonStyleClass:'p-button-danger',
      accept: () => this.runRowAction(item,
        this.service.delete(item.id, item.version), 'Tetkik kaydı silindi.')
    });
  }
  private runRowAction(item: ProcedureDefItem, request: Observable<unknown>, message: string): void {
    if (this.actionLoadingId()) return;
    this.actionLoadingId.set(item.id);
    request.pipe(finalize(() => this.actionLoadingId.set(null))).subscribe({
      next: () => { this.successMessage.set(message); this.loadRecords(); },
      error: (err: HttpErrorResponse) => this.errorMessage.set(this.errorText(err))
    });
  }
  public openAudit(item: ProcedureDefItem): void { if (this.canAudit()) this.auditRecord.set(item); }
  private errorText(err: HttpErrorResponse): string {
    if (typeof err.error?.detail === 'string' && err.error.detail.trim()) return err.error.detail;
    if (err.status === 403) return 'Bu işlem için yetkiniz yok.';
    if (err.status === 404) return 'Tetkik kaydı bulunamadı.';
    if (err.status === 409) return 'Kayıt değişmiş veya kod başka bir kayıtta kullanılıyor. Listeyi yenileyiniz.';
    return 'İşlem tamamlanamadı, tekrar deneyiniz.';
  }
}
