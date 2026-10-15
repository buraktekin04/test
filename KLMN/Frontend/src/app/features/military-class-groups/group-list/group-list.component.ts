import { Component, computed, inject, OnInit, signal } from '@angular/core';
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
import { MilitaryClassGroupItem, MilitaryClassGroupFields, MilitaryClassGroupQuery, MilitaryClassGroupOption } from '../models/military-class-groups.model';
import { MilitaryClassGroupService } from '../services/military-class-groups.service';

type FormField = 'code' | 'name' | 'shortName' | 'forcesCommandId' | 'validFrom' | 'validTo';

/** Askerî Sınıf Grubu ekranı; izin bazlı CRUD, Excel, audit ve PrimeNG client-side sıralama. */
@Component({
  selector: 'app-military-class-groups-list',
  standalone: true,
  imports: [DatePipe, ReactiveFormsModule, ButtonModule, CheckboxModule,
    ConfirmDialogModule, DialogModule, InputTextModule, TableModule, TagModule, AuditHistoryComponent],
  providers: [ConfirmationService],
  templateUrl: './group-list.component.html',
  styleUrl: './group-list.component.scss'
})
export class MilitaryClassGroupListComponent implements OnInit {
  private readonly api = inject(MilitaryClassGroupService);
  private readonly fb = inject(FormBuilder);
  private readonly permissions = inject(PermissionService);
  private readonly confirmation = inject(ConfirmationService);

  public readonly canCreate = computed(() => this.permissions.has(PermissionCodes.MilitaryClassGroups.Create));
  public readonly canUpdate = computed(() => this.permissions.has(PermissionCodes.MilitaryClassGroups.Update));
  public readonly canDelete = computed(() => this.permissions.has(PermissionCodes.MilitaryClassGroups.Delete));
  public readonly canExport = computed(() => this.permissions.has(PermissionCodes.MilitaryClassGroups.Query));
  public readonly canAudit = computed(() => this.permissions.has(PermissionCodes.AuditLogs.View));
  public readonly records = signal<MilitaryClassGroupItem[]>([]);
  public readonly totalCount = signal(0);
  public readonly pageNumber = signal(1);
  public readonly pageSize = signal(20);
  public readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));
  public readonly loading = signal(false);
  public readonly exporting = signal(false);
  public readonly saving = signal(false);
  public readonly actionLoadingId = signal<string | null>(null);
  public readonly errorMessage = signal<string | null>(null);
  public readonly successMessage = signal<string | null>(null);
  public readonly dialogVisible = signal(false);
  public readonly editing = signal<MilitaryClassGroupItem | null>(null);
  public readonly auditRecord = signal<MilitaryClassGroupItem | null>(null);
  public readonly formError = signal<string | null>(null);
  public readonly fieldServerErrors = signal<Record<string, string>>({});
  public readonly attemptedSave = signal(false);
  /** Formlar sadece kendi modül izinleriyle üst tanımların kod/adlarını okur. */
  public readonly forcesOptions = signal<MilitaryClassGroupOption[]>([]);

  public readonly optionsLoading = signal(false);
  public readonly optionsError = signal<string | null>(null);
  public readonly filterForm = this.fb.nonNullable.group({
    search: '', includeInactive: false
  });
  public readonly columnFilterForm = this.fb.nonNullable.group({
    isActive: '', forcesCommandId: ''
  });
  public readonly itemForm = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.maxLength(100), Validators.pattern(/^[A-Za-z0-9_-]+$/)]],
    name: ['', [Validators.required, Validators.maxLength(250)]],
    shortName: ['', Validators.maxLength(100)],
    forcesCommandId: ['', Validators.required],
    validFrom: [''],
    validTo: ['']
  });
  private listRequestId = 0;
  private dialogRequestId = 0;
  private optionsRequestId = 0;

  public ngOnInit(): void { this.loadRecords(); this.loadOptions(); }

  /** Grup formunda kuvvetleri, düzenlemede seçili pasif kuvvet dahil yükler. */
  public loadOptions(selectedForceId?: string | null): void {
    const requestId = ++this.optionsRequestId;
    this.optionsLoading.set(true);
    this.optionsError.set(null);
    this.api.options(selectedForceId).pipe(finalize(() => {
      if (requestId === this.optionsRequestId) this.optionsLoading.set(false);
    })).subscribe({
      next: result => { if (requestId === this.optionsRequestId) this.forcesOptions.set(result); },
      error: (err: HttpErrorResponse) => {
        if (requestId === this.optionsRequestId) this.optionsError.set(this.errorText(err));
      }
    });
  }

  private buildQuery(): MilitaryClassGroupQuery {
    const q = this.filterForm.getRawValue();
    const f = this.columnFilterForm.getRawValue();
    const isActive = f.isActive === 'true' ? true : f.isActive === 'false' ? false : null;
    return {
      search: q.search,
      includeInactive: q.includeInactive || isActive === false,
      isActive,
      forcesCommandId: f.forcesCommandId || null,

      pageNumber: this.pageNumber(), pageSize: this.pageSize()
    };
  }
  /** Backend filtreleme/sayfalama; geç dönen önceki isteği yoksayar. */
  public loadRecords(): void {
    const id = ++this.listRequestId;
    this.loading.set(true);
    this.errorMessage.set(null);
    this.api.list(this.buildQuery()).pipe(finalize(() => {
      if (id === this.listRequestId) this.loading.set(false);
    })).subscribe({
      next: result => {
        if (id !== this.listRequestId) return;
        this.records.set([...result.items]); this.totalCount.set(result.totalCount);
      },
      error: (err: HttpErrorResponse) => {
        if (id !== this.listRequestId) return;
        this.errorMessage.set(this.errorText(err)); this.records.set([]);
        this.totalCount.set(0);
      }
    });
  }
  public search(): void { this.pageNumber.set(1); this.loadRecords(); }
  public clearFilters(): void {
    this.filterForm.reset({ search:'', includeInactive:false });
    this.columnFilterForm.reset({ isActive:'', forcesCommandId:'' });
    this.pageNumber.set(1); this.loadRecords();
  }
  public onPageSizeChange(event: Event): void {
    const size = Number((event.target as HTMLSelectElement).value);
    if (![10,20,50,100].includes(size)) return;
    this.pageSize.set(size); this.pageNumber.set(1); this.loadRecords();
  }
  public goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || this.loading()) return;
    this.pageNumber.set(page); this.loadRecords();
  }

  /** Excel yalnızca Query yetkisiyle API üzerinden oluşturulur. */
  public exportExcel(): void {
    if (!this.canExport() || this.exporting()) return;
    this.exporting.set(true);
    this.api.export(this.buildQuery()).pipe(finalize(() => this.exporting.set(false))).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement('a'); anchor.href = url;
        anchor.download = 'military-class-groups_' + new Date().toISOString().slice(0, 10) + '.xlsx';
        anchor.click(); setTimeout(() => URL.revokeObjectURL(url), 1000);
        this.successMessage.set('Excel dosyası indirildi.');
      },
      error: (err: HttpErrorResponse) => this.errorMessage.set(this.errorText(err))
    });
  }
  /** Create popup'ında eski alan hatalarını temizler. */
  public openCreateDialog(): void {
    if (!this.canCreate()) return;
    ++this.dialogRequestId;
    this.editing.set(null);
    this.itemForm.reset({ code:'', name:'', shortName:'', forcesCommandId:'', validFrom:'', validTo:'' });
    this.attemptedSave.set(false); this.fieldServerErrors.set({});
    this.formError.set(null); this.dialogVisible.set(true);

  }
  /** Güncel xmin ile düzenleme popup'ını açar. */
  public openEditDialog(row: MilitaryClassGroupItem): void {
    if (!this.canUpdate() || this.actionLoadingId()) return;
    const id = ++this.dialogRequestId;
    this.actionLoadingId.set(row.id);
    this.api.getById(row.id).pipe(finalize(() => {
      if (id === this.dialogRequestId) this.actionLoadingId.set(null);
    })).subscribe({
      next: detail => {
        if (id !== this.dialogRequestId) return;
        this.editing.set(detail);
        this.itemForm.reset({
          code: detail.code, name: detail.name, shortName: detail.shortName ?? '',
          forcesCommandId: detail.forcesCommandId,
          validFrom: ValidityPeriodUtils.toLocalInput(detail.validFrom),
          validTo: ValidityPeriodUtils.toLocalInput(detail.validTo)
        });
        this.loadOptions(detail.forcesCommandId);
        this.fieldServerErrors.set({}); this.formError.set(null);
        this.attemptedSave.set(false); this.dialogVisible.set(true);
      },
      error: (err: HttpErrorResponse) => {
        if (id === this.dialogRequestId) this.errorMessage.set(this.errorText(err));
      }
    });
  }
  public onDialogVisibleChange(visible: boolean): void {
    if (!visible && this.saving()) return;
    this.dialogVisible.set(visible);
    if (!visible) ++this.dialogRequestId;
  }

  /** Alan hatasını ilgili input altında gösterir. */
  public fieldError(field: FormField): string | null {
    if (this.fieldServerErrors()[field]) return this.fieldServerErrors()[field];
    const control = this.itemForm.controls[field];
    if (!this.attemptedSave() && !control.touched) return null;
    if (control.hasError('required')) return 'Bu alan zorunludur.';
    if (control.hasError('maxlength')) return 'Karakter sınırı aşıldı.';
    if (control.hasError('pattern')) return 'Kod yalnızca harf, rakam, - ve _ içerebilir.';
    if (field === 'validTo') {
      const values = this.itemForm.getRawValue();
      if (!ValidityPeriodUtils.hasValidRange(values.validFrom, values.validTo))
        return 'Bitiş başlangıçtan sonra olmalıdır.';
    }

    return null;
  }
  private focusField(field: string): void {
    requestAnimationFrame(() => {
      const element = document.getElementById('military-class-groups-' + field);
      element?.scrollIntoView({ behavior:'smooth', block:'center' });
      element?.focus({ preventScroll:true });
    });
  }

  /** Kaydetmede izin, alan, referans ve concurrency kuralları API'de de uygulanır. */
  public save(): void {
    if (this.saving() || (this.editing() ? !this.canUpdate() : !this.canCreate())) return;
    this.attemptedSave.set(true); this.formError.set(null); this.fieldServerErrors.set({});
    if (this.itemForm.invalid) {
      this.itemForm.markAllAsTouched();
      const first = (["code","name","shortName","forcesCommandId","validFrom","validTo"] as FormField[]).find(x => this.itemForm.controls[x].invalid);
      if (first) this.focusField(first);
      return;
    }
    const values = this.itemForm.getRawValue();
    if (!ValidityPeriodUtils.hasValidRange(values.validFrom, values.validTo)) {
      this.focusField('validTo'); return;
    }

    if (this.optionsLoading() || this.optionsError()) {
      this.formError.set('Seçim listelerini kontrol ederek tekrar deneyiniz.'); return;
    }

    const fields: MilitaryClassGroupFields = {
      forcesCommandId: values.forcesCommandId,
      code: values.code.trim().toUpperCase(), name: values.name.trim(),
      shortName: values.shortName.trim() || null,
      ...ValidityPeriodUtils.fromLocalInputs(values.validFrom, values.validTo)
    };
    const editing = this.editing();
    const request: Observable<unknown> = editing
      ? this.api.update({ ...fields, id:editing.id, version:editing.version })
      : this.api.create(fields);
    this.saving.set(true);
    request.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.dialogVisible.set(false); ++this.dialogRequestId;
        this.editing.set(null);
        this.successMessage.set(editing ? 'Kayıt güncellendi.' : 'Kayıt oluşturuldu.');
        this.loadRecords();
      },
      error: (err: HttpErrorResponse) => this.handleSaveError(err)
    });
  }
  /** FluentValidation alan hatalarını popup içinde görünür alana taşır. */
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
    const first = (["code","name","shortName","forcesCommandId","validFrom","validTo"] as FormField[]).find(key => mapped[key]);
    if (first) {
      this.formError.set('İşaretlenen alanları kontrol ediniz.');
      this.focusField(first);
    } else this.formError.set(this.errorText(err));
  }

  /** Yalnızca Update izni, BaseEntity.IsActive durumunu değiştirir. */
  public confirmStatusChange(item: MilitaryClassGroupItem): void {
    if (!this.canUpdate() || this.actionLoadingId()) return;
    this.confirmation.confirm({
      header:'Aktif/Pasif', message:'Kayıt ' + (item.isActive ? 'pasifleştirilsin' : 'aktifleştirilsin') + ' mi?',
      acceptLabel:'Evet', rejectLabel:'Vazgeç',
      accept: () => this.runRowAction(item,
        this.api.changeStatus({ id:item.id, version:item.version, isActive:!item.isActive }),
        'Kayıt durumu değiştirildi.')
    });
  }
  public confirmDelete(item: MilitaryClassGroupItem): void {
    if (!this.canDelete() || this.actionLoadingId()) return;
    this.confirmation.confirm({
      header:'Sil', message:'Bu kayıt silinsin mi?', acceptLabel:'Sil', rejectLabel:'Vazgeç',
      acceptButtonStyleClass:'p-button-danger',
      accept: () => this.runRowAction(item, this.api.delete(item.id, item.version), 'Kayıt silindi.')
    });
  }
  private runRowAction(item: MilitaryClassGroupItem, request: Observable<unknown>, message: string): void {
    if (this.actionLoadingId()) return;
    this.actionLoadingId.set(item.id);
    request.pipe(finalize(() => this.actionLoadingId.set(null))).subscribe({
      next: () => { this.successMessage.set(message); this.loadRecords(); },
      error: (err: HttpErrorResponse) => this.errorMessage.set(this.errorText(err))
    });
  }
  public openAudit(item: MilitaryClassGroupItem): void {
    if (this.canAudit()) this.auditRecord.set(item);
  }
  private errorText(err: HttpErrorResponse): string {
    if (typeof err.error?.detail === 'string' && err.error.detail.trim()) return err.error.detail;
    if (err.status === 403) return 'Bu işlem için yetkiniz yok.';
    if (err.status === 404) return 'Kayıt bulunamadı.';
    if (err.status === 409) return 'Kayıt değişmiş, mükerrer veya ilişkili olabilir; listeyi yenileyiniz.';
    return 'İşlem tamamlanamadı, tekrar deneyiniz.';
  }
}
