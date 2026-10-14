import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { ValidityPeriodUtils } from '../../../core/utils/validity-period.utils';
import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, Observable } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { PermissionCodes } from '../../../core/permissions/constants/permission-codes';
import { PermissionService } from '../../../core/permissions/services/permission.service';
import { AuditHistoryComponent } from '../../../shared/audit-history/audit-history.component';
import {
  PurchaseNumberDefFields,
  PurchaseNumberDefItem,
  PurchaseNumberDefMutationResponse,
  PurchaseNumberDefQuery
} from '../models/purchase-number-def.model';
import { PurchaseNumberService } from '../services/purchase-number.service';

type PurchaseNumberFormField = 'code' | 'name' | 'supplyDescriptionName' |
  'validFrom' | 'validTo' | 'healthReportEndDate';

/** Alım numarası tanımları: liste, filtre, CQRS işlemleri ve kayıt geçmişi. */
@Component({
  selector: 'app-purchase-number-list',
  standalone: true,
  imports: [
    DatePipe, ReactiveFormsModule, ButtonModule, CheckboxModule,
    ConfirmDialogModule, DialogModule, InputTextModule,
    MessageModule, SelectModule, TableModule, TagModule, AuditHistoryComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './purchase-number-list.component.html',
  styleUrl: './purchase-number-list.component.scss'
})
export class PurchaseNumberListComponent implements OnInit {
  private readonly service = inject(PurchaseNumberService);
  private readonly fb = inject(FormBuilder);
  private readonly permissionService = inject(PermissionService);
  private readonly confirmations = inject(ConfirmationService);

  /** Excel için ayrıca sorgulama yetkisi gereklidir. */
  public readonly canExport = computed(() =>
    this.permissionService.has(PermissionCodes.PurchaseNumbers.Query));

  /** Menü ve satır butonlarındaki UI görünürlük kontrolleridir; API ayrıca korunur. */
  public readonly canCreate = computed(
    () => this.permissionService.has(PermissionCodes.PurchaseNumbers.Create));
  public readonly canUpdate = computed(
    () => this.permissionService.has(PermissionCodes.PurchaseNumbers.Update));
  public readonly canDelete = computed(
    () => this.permissionService.has(PermissionCodes.PurchaseNumbers.Delete));
  public readonly canViewAudit = computed(
    () => this.permissionService.has(PermissionCodes.AuditLogs.View));

  public readonly records = signal<PurchaseNumberDefItem[]>([]);
  public readonly totalCount = signal(0);
  public readonly pageNumber = signal(1);
  public readonly pageSize = signal(20);
  public readonly loading = signal(false);
  public readonly exporting = signal(false);
  public readonly actionLoadingId = signal<string | null>(null);
  public readonly successMessage = signal<string | null>(null);
  public readonly errorMessage = signal<string | null>(null);
  public readonly columnFiltersExpanded = signal(false);
  public readonly auditRecord = signal<PurchaseNumberDefItem | null>(null);

  private listRequestVersion = 0;
  private dialogRequestVersion = 0;

  /** Liste ve Excel aramalarının ortak filtre durumudur. */
  public readonly filterForm = this.fb.nonNullable.group({
    search: '',
    includeInactive: false
  });

  /** Filtreler API'de tüm kayıtlar üzerinde çalışır. */
  public readonly columnFilterForm = this.fb.group({
    isActive: this.fb.control<boolean | null>(null),
    supplyDescription: this.fb.nonNullable.control('')
  });

  public readonly dialogVisible = signal(false);
  public readonly editingRecord = signal<PurchaseNumberDefItem | null>(null);
  public readonly formError = signal<string | null>(null);
  public readonly saving = signal(false);

  /** Uzun form yerine iki sade bölüm ve alan bazlı kısa doğrulama mesajları. */
  public readonly attemptedSave = signal(false);
  public readonly serverErrors = signal<Record<string, string>>({});
  public readonly purchaseNumberForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(250)]],
    code: ['', [Validators.required, Validators.maxLength(100),
      Validators.pattern(/^[A-Za-z0-9_-]+$/)]],
    supplyDescriptionName: ['', [Validators.required, Validators.maxLength(500)]],
    validFrom: [''],
    validTo: [''],
    healthReportEndDate: ['']
  });

  /** İlgili input altında kısa hata metni gösterir. */
  public fieldError(field: PurchaseNumberFormField): string | null {
    if (this.serverErrors()[field]) return this.serverErrors()[field];
    const control = this.purchaseNumberForm.controls[field];
    if (!this.attemptedSave() && !control.touched) return null;
    if (control.hasError('required')) return 'Bu alan zorunludur.';
    if (control.hasError('maxlength')) return 'Karakter sınırı aşıldı.';
    if (control.hasError('pattern')) return 'Yalnızca harf, sayı, tire ve alt çizgi kullanınız.';
    if (field === 'validTo') {
      const f = this.purchaseNumberForm.controls;
      if (!ValidityPeriodUtils.hasValidRange(f.validFrom.value, f.validTo.value))
        return 'Bitiş tarihi başlangıçtan sonra olmalıdır.';
    }
    return null;
  }

  /** Kaydet sonrası ilk sorunlu alanı görünür yapar. */
  private focusField(field: string): void {
    requestAnimationFrame(() => {
      if (!this.dialogVisible()) return;
      const input = document.getElementById('purchase-' + field);
      input?.scrollIntoView({ behavior: 'smooth', block: 'center' });
      input?.focus({ preventScroll: true });
    });
  }

  public readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));
  public readonly rangeStart = computed(() =>
    this.totalCount() === 0 ? 0 : (this.pageNumber() - 1) * this.pageSize() + 1);
  public readonly rangeEnd = computed(() =>
    Math.min(this.totalCount(), this.pageNumber() * this.pageSize()));

  /** Kaydetme bildirimleri kısa süre sonra otomatik kapanır. */
  public constructor() {
    effect(onCleanup => {
      if (!this.successMessage()) return;
      const timer = setTimeout(() => this.successMessage.set(null), 10_000);
      onCleanup(() => clearTimeout(timer));
    });
    this.purchaseNumberForm.valueChanges.pipe(takeUntilDestroyed()).subscribe(() => {
      this.formError.set(null);
      this.serverErrors.set({});
    });
  }

  public ngOnInit(): void { this.loadRecords(); }

  private buildQuery(): PurchaseNumberDefQuery {
    const form = this.filterForm.getRawValue();
    const columns = this.columnFilterForm.getRawValue();
    return {
      search: form.search,
      includeInactive: form.includeInactive || columns.isActive === false,
      isActive: columns.isActive,
      supplyDescription: columns.supplyDescription,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    };
  }

  /** Sunucudan rol ekranındaki gibi filtreli ve sayfalı sonuçları yükler. */
  public loadRecords(): void {
    const version = ++this.listRequestVersion;
    this.loading.set(true);
    this.errorMessage.set(null);
    this.service.list(this.buildQuery())
      .pipe(finalize(() => {
        if (version === this.listRequestVersion) this.loading.set(false);
      }))
      .subscribe({
        next: result => {
          if (version !== this.listRequestVersion) return;
          this.records.set(result.items);
          this.totalCount.set(result.totalCount);
        },
        error: (error: HttpErrorResponse) => {
          if (version !== this.listRequestVersion) return;
          this.records.set([]);
          this.totalCount.set(0);
          this.errorMessage.set(this.errorText(error));
        }
      });
  }

  public search(): void { this.pageNumber.set(1); this.loadRecords(); }
  public applyColumnFilters(): void { this.pageNumber.set(1); this.loadRecords(); }
  public toggleColumnFilters(): void {
    this.columnFiltersExpanded.update(value => !value);
  }
  public clearFilters(): void {
    this.filterForm.reset({ search: '', includeInactive: false });
    this.columnFilterForm.reset({ isActive: null, supplyDescription: '' });
    this.pageNumber.set(1);
    this.loadRecords();
  }

  public goToPage(page: number): void {
    if (page === this.pageNumber() || page < 1 ||
        page > this.totalPages() || this.loading()) return;
    this.pageNumber.set(page);
    this.loadRecords();
  }

  public onPageSizeChange(event: Event): void {
    const size = Number((event.target as HTMLSelectElement).value);
    if (![10, 20, 50, 100].includes(size) || size === this.pageSize()) return;
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.loadRecords();
  }

  /** Listeyi değil, sunucudaki tüm filtrelenmiş kayıtları Excel'e aktarır. */
  public exportExcel(): void {
    if (!this.canExport() || this.exporting()) return;
    this.exporting.set(true);
    this.service.export(this.buildQuery())
      .pipe(finalize(() => this.exporting.set(false)))
      .subscribe({
        next: blob => {
          const url = URL.createObjectURL(blob);
          const anchor = document.createElement('a');
          anchor.href = url;
          anchor.download = 'Alim_Numarasi_Tanimlari_' +
            new Date().toISOString().slice(0, 10) + '.xlsx';
          anchor.click();
          setTimeout(() => URL.revokeObjectURL(url), 1000);
          this.successMessage.set('Alım numarası listesi Excel olarak indirildi.');
        },
        error: (error: HttpErrorResponse) =>
          this.errorMessage.set(this.errorText(error))
      });
  }

  /** Yeni alım numarası için boş tanım formunu açar. */
  public openCreateDialog(): void {
    if (!this.canCreate() || this.saving() || this.actionLoadingId()) return;
    ++this.dialogRequestVersion;
    this.editingRecord.set(null);
    this.purchaseNumberForm.reset({
      name: '', code: '', supplyDescriptionName: '',
      validFrom: '', validTo: '', healthReportEndDate: ''
    });
    this.formError.set(null);
    this.serverErrors.set({});
    this.attemptedSave.set(false);
    this.dialogVisible.set(true);
  }

  /** Düzenleme öncesinde güncel xmin ve alanları getirir. */
  public openEditDialog(record: PurchaseNumberDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId() || this.saving()) return;
    const version = ++this.dialogRequestVersion;
    this.actionLoadingId.set(record.id);
    this.formError.set(null);
    this.service.getById(record.id).pipe(finalize(() => {
      if (version === this.dialogRequestVersion) this.actionLoadingId.set(null);
    })).subscribe({
      next: detail => {
        if (version !== this.dialogRequestVersion) return;
        this.editingRecord.set(detail);
        this.purchaseNumberForm.reset({
          name: detail.name, code: detail.code,
          supplyDescriptionName: detail.supplyDescriptionName,
          validFrom: ValidityPeriodUtils.toLocalInput(detail.validFrom),
          validTo: ValidityPeriodUtils.toLocalInput(detail.validTo),
          healthReportEndDate: ValidityPeriodUtils.toLocalInput(detail.healthReportEndDate)
        });
        this.attemptedSave.set(false);
        this.serverErrors.set({});
        this.dialogVisible.set(true);
      },
      error: (error: HttpErrorResponse) => {
        if (version === this.dialogRequestVersion) this.errorMessage.set(this.errorText(error));
      }
    });
  }

  /** Modal X veya Vazgeç işleminde eski bekleyen API cevaplarını geçersiz kılar. */
  public onDialogVisibleChange(visible: boolean): void {
    if (!visible && this.saving()) return;
    this.dialogVisible.set(visible);
    if (!visible) ++this.dialogRequestVersion;
  }

  /** Fotoğraftaki alım alanlarını UTC'ye dönüştürüp CQRS endpointine gönderir. */
  public saveRecord(): void {
    if (this.saving() || (this.editingRecord() ? !this.canUpdate() : !this.canCreate()))
      return;

    this.attemptedSave.set(true);
    this.formError.set(null);
    this.serverErrors.set({});
    if (this.purchaseNumberForm.invalid) {
      this.purchaseNumberForm.markAllAsTouched();
      const field = (['name', 'code', 'supplyDescriptionName', 'validFrom', 'validTo'] as const)
        .find(x => this.purchaseNumberForm.controls[x].invalid);
      if (field) this.focusField(field);
      return;
    }

    const values = this.purchaseNumberForm.getRawValue();
    if (!ValidityPeriodUtils.hasValidRange(values.validFrom, values.validTo)) {
      this.focusField('validTo');
      return;
    }
    const fields: PurchaseNumberDefFields = {
      name: values.name.trim(),
      code: values.code.trim().toUpperCase(),
      supplyDescriptionName: values.supplyDescriptionName.trim(),
      ...ValidityPeriodUtils.fromLocalInputs(values.validFrom, values.validTo),
      healthReportEndDate: ValidityPeriodUtils.toUtcIso(values.healthReportEndDate)
    };
    if (!fields.name || !fields.code || !fields.supplyDescriptionName) {
      this.formError.set('Alım adı, kod ve temin tanım adı zorunludur.');
      return;
    }

    const edited = this.editingRecord();
    const req: Observable<PurchaseNumberDefMutationResponse> = edited
      ? this.service.update({ ...fields, id: edited.id, version: edited.version })
      : this.service.create(fields);

    this.saving.set(true);
    this.formError.set(null);
    req.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        // Yanıt geldiğinde saving hâlâ true olabilir; kapatma guard'ını atlamadan
        // formu programatik olarak kapatıp eski dropdown sorgularını geçersiz kıl.
        this.dialogVisible.set(false);
        ++this.dialogRequestVersion;
        this.editingRecord.set(null);
        this.attemptedSave.set(false);
        this.successMessage.set(edited ? 'Alım numarası güncellendi.' : 'Yeni alım numarası oluşturuldu.');
        this.loadRecords();
      },
      error: (error: HttpErrorResponse) => this.handleSaveError(error)
    });
  }

  private handleSaveError(error: HttpErrorResponse): void {
    const errors: unknown = error.error?.errors;
    const mapped: Record<string, string> = {};
    if (errors && typeof errors === 'object' && !Array.isArray(errors)) {
      for (const [key, value] of Object.entries(errors)) {
        const name = key.charAt(0).toLowerCase() + key.slice(1);
        if (Array.isArray(value) && typeof value[0] === 'string')
          mapped[name] = value[0];
      }
    }
    this.serverErrors.set(mapped);
    const first = ['name', 'code', 'supplyDescriptionName', 'validFrom', 'validTo', 'healthReportEndDate']
      .find(field => mapped[field]);
    if (first) {
      this.formError.set('İşaretli alanları kontrol edip yeniden kaydediniz.');
      this.focusField(first);
    } else {
      this.formError.set(this.errorText(error));
    }
  }

  /** Aktiflik değişikliği için onay ister. */
  public confirmStatusChange(record: PurchaseNumberDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId()) return;
    this.confirmations.confirm({
      header: 'Alım Numarası Durumu',
      message: record.name + ' kaydını ' +
        (record.isActive ? 'pasifleştirmek' : 'aktifleştirmek') + ' istiyor musunuz?',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet',
      rejectLabel: 'Vazgeç',
      accept: () => this.runRowAction(record,
        this.service.changeStatus({
          id: record.id, version: record.version, isActive: !record.isActive
        }),
        'Alım numarası durumu değiştirildi.')
    });
  }

  /** Alım numarasını soft-delete etmek için onay ister. */
  public confirmDelete(record: PurchaseNumberDefItem): void {
    if (!this.canDelete() || this.actionLoadingId()) return;
    this.confirmations.confirm({
      header: 'Alım Numarasını Sil',
      message: record.name + ' silinsin mi? Bu işlem kaydı arşivler.',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.runRowAction(record,
        this.service.delete(record.id, record.version), 'Alım numarası silindi.')
    });
  }

  private runRowAction(record: PurchaseNumberDefItem, request: Observable<unknown>,
                       success: string): void {
    if (this.actionLoadingId()) return;
    this.actionLoadingId.set(record.id);
    request.pipe(finalize(() => this.actionLoadingId.set(null))).subscribe({
      next: () => { this.successMessage.set(success); this.loadRecords(); },
      error: (error: HttpErrorResponse) => this.errorMessage.set(this.errorText(error))
    });
  }

  public openAudit(record: PurchaseNumberDefItem): void {
    if (this.canViewAudit()) this.auditRecord.set(record);
  }

  /** Merkezi ASP.NET ProblemDetails yanıtını kullanıcı dostu hale getirir. */
  private errorText(error: HttpErrorResponse): string {
    const detail = error.error?.detail;
    if (typeof detail === 'string' && detail.trim()) return detail;
    if (error.status === 409)
      return 'Kayıt başka bir kullanıcı tarafından değiştirilmiş olabilir. Listeyi yenileyiniz.';
    if (error.status === 403) return 'Bu işlem için yetkiniz bulunmuyor.';
    if (error.status === 404) return 'Alım numarası bulunamadı. Listeyi yenileyiniz.';
    return 'İşlem tamamlanamadı. Lütfen tekrar deneyiniz.';
  }
}
