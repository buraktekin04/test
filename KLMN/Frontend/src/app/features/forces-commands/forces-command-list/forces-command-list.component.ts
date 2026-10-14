import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { ValidityPeriodUtils } from '../../../core/utils/validity-period.utils';
import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, Observable } from 'rxjs';

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
  ForcesCommandDefFields,
  ForcesCommandDefItem,
  ForcesCommandDefMutationResponse,
  ForcesCommandDefQuery
} from '../models/forces-command-def.model';
import { ForcesCommandService } from '../services/forces-command.service';

/** Birliklerin bağlandığı kuvvet komutanlığı tanımlarını yönetir. */
@Component({
  selector: 'app-forces-command-list',
  standalone: true,
  imports: [
    DatePipe, ReactiveFormsModule, ButtonModule, CheckboxModule,
    ConfirmDialogModule, DialogModule, InputTextModule,
    MessageModule, SelectModule, TableModule, TagModule, AuditHistoryComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './forces-command-list.component.html',
  styleUrl: './forces-command-list.component.scss'
})
export class ForcesCommandListComponent implements OnInit {
  private readonly service = inject(ForcesCommandService);
  private readonly fb = inject(FormBuilder);
  private readonly permissionService = inject(PermissionService);
  private readonly confirmations = inject(ConfirmationService);

  /** Menü ve satır butonlarındaki UI görünürlük kontrolleridir; API ayrıca korunur. */
  public readonly canCreate = computed(
    () => this.permissionService.has(PermissionCodes.ForcesCommands.Create));
  public readonly canUpdate = computed(
    () => this.permissionService.has(PermissionCodes.ForcesCommands.Update));
  public readonly canDelete = computed(
    () => this.permissionService.has(PermissionCodes.ForcesCommands.Delete));
  public readonly canViewAudit = computed(
    () => this.permissionService.has(PermissionCodes.AuditLogs.View));

  public readonly units = signal<ForcesCommandDefItem[]>([]);
  public readonly totalCount = signal(0);
  public readonly pageNumber = signal(1);
  public readonly pageSize = signal(20);
  public readonly loading = signal(false);
  public readonly exporting = signal(false);
  public readonly actionLoadingId = signal<string | null>(null);
  public readonly successMessage = signal<string | null>(null);
  public readonly errorMessage = signal<string | null>(null);
  public readonly columnFiltersExpanded = signal(false);
  public readonly auditUnit = signal<ForcesCommandDefItem | null>(null);

  private listRequestVersion = 0;
  private dialogRequestVersion = 0;

  /** Liste ve Excel aramalarının ortak filtre durumudur. */
  public readonly filterForm = this.fb.nonNullable.group({
    search: '',
    includeInactive: false
  });

  /** Kullanıcı ekranındaki aynı kompakt filtre bileşenleri kullanılır. */
  public readonly columnFilterForm = this.fb.group({
    isActive: this.fb.control<boolean | null>(null),
  });

  public readonly dialogVisible = signal(false);
  public readonly editingUnit = signal<ForcesCommandDefItem | null>(null);
  public readonly formError = signal<string | null>(null);
  public readonly saving = signal(false);

  /** Geçerli entity'deki Code ve Name veri tipi/uzunluklarıyla eşleşen form. */
  public readonly forcesCommandForm = this.fb.group({
    name: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(250)]),
    code: this.fb.nonNullable.control('', [
      Validators.required, Validators.maxLength(100),
      Validators.pattern(/^[A-Za-z0-9_-]+$/)
    ]),
    validFrom: this.fb.nonNullable.control(''),
    validTo: this.fb.nonNullable.control(''),
  });

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
  }

  public ngOnInit(): void { this.loadUnits(); }

  private buildQuery(): ForcesCommandDefQuery {
    const form = this.filterForm.getRawValue();
    const columns = this.columnFilterForm.getRawValue();
    return {
      search: form.search,
      includeInactive: form.includeInactive || columns.isActive === false,
      isActive: columns.isActive,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    };
  }

  /** Sunucudan rol ekranındaki gibi filtreli ve sayfalı sonuçları yükler. */
  public loadUnits(): void {
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
          this.units.set(result.items);
          this.totalCount.set(result.totalCount);
        },
        error: (error: HttpErrorResponse) => {
          if (version !== this.listRequestVersion) return;
          this.units.set([]);
          this.totalCount.set(0);
          this.errorMessage.set(this.errorText(error));
        }
      });
  }

  public search(): void { this.pageNumber.set(1); this.loadUnits(); }
  public applyColumnFilters(): void { this.pageNumber.set(1); this.loadUnits(); }
  public toggleColumnFilters(): void {
    this.columnFiltersExpanded.update(value => !value);
  }
  public clearFilters(): void {
    this.filterForm.reset({ search: '', includeInactive: false });
    this.columnFilterForm.reset({ isActive: null });
    this.pageNumber.set(1);
    this.loadUnits();
  }

  public goToPage(page: number): void {
    if (page === this.pageNumber() || page < 1 ||
        page > this.totalPages() || this.loading()) return;
    this.pageNumber.set(page);
    this.loadUnits();
  }

  public onPageSizeChange(event: Event): void {
    const size = Number((event.target as HTMLSelectElement).value);
    if (![10, 20, 50, 100].includes(size) || size === this.pageSize()) return;
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.loadUnits();
  }

  /** Listeyi değil, sunucudaki tüm filtrelenmiş kayıtları Excel'e aktarır. */
  public exportExcel(): void {
    if (this.exporting()) return;
    this.exporting.set(true);
    this.service.export(this.buildQuery())
      .pipe(finalize(() => this.exporting.set(false)))
      .subscribe({
        next: blob => {
          const url = URL.createObjectURL(blob);
          const anchor = document.createElement('a');
          anchor.href = url;
          anchor.download = 'Kuvvet_Komutanligi_Tanimlari_' +
            new Date().toISOString().slice(0, 10) + '.xlsx';
          anchor.click();
          setTimeout(() => URL.revokeObjectURL(url), 1000);
          this.successMessage.set('Kuvvet komutanlığı listesi Excel olarak indirildi.');
        },
        error: (error: HttpErrorResponse) =>
          this.errorMessage.set(this.errorText(error))
      });
  }

  /** Yeni kuvvet komutanlığı için boş tanım formunu açar. */
  public openCreateDialog(): void {
    if (!this.canCreate() || this.saving() || this.actionLoadingId()) return;
    ++this.dialogRequestVersion;
    this.editingUnit.set(null);
    this.forcesCommandForm.reset({ name: '', code: '', validFrom: '', validTo: '' });
    this.formError.set(null);
    this.dialogVisible.set(true);
  }

  /** Düzenleme öncesinde güncel xmin ve alanları getirir. */
  public openEditDialog(unit: ForcesCommandDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId() || this.saving()) return;
    const version = ++this.dialogRequestVersion;
    this.actionLoadingId.set(unit.id);
    this.formError.set(null);
    this.service.getById(unit.id).pipe(finalize(() => {
      if (version === this.dialogRequestVersion) this.actionLoadingId.set(null);
    })).subscribe({
      next: detail => {
        if (version !== this.dialogRequestVersion) return;
        this.editingUnit.set(detail);
        this.forcesCommandForm.reset({
          name: detail.name, code: detail.code,
          validFrom: ValidityPeriodUtils.toLocalInput(detail.validFrom),
          validTo: ValidityPeriodUtils.toLocalInput(detail.validTo)
        });
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

  /** Ad, kod ve geçerlilik tarihlerini bağımsız CQRS endpointine gönderir. */
  public saveUnit(): void {
    if (this.saving() || (this.editingUnit() ? !this.canUpdate() : !this.canCreate()))
      return;

    if (this.forcesCommandForm.invalid) {
      this.forcesCommandForm.markAllAsTouched();
      this.formError.set('Kuvvet komutanlığı adını ve kodunu kontrol ediniz.');
      return;
    }

    const values = this.forcesCommandForm.getRawValue();
    if (!ValidityPeriodUtils.hasValidRange(values.validFrom, values.validTo)) {
      this.formError.set('Bitiş tarihi başlangıç tarihinden sonra olmalıdır.');
      return;
    }
    const fields: ForcesCommandDefFields = {
      ...ValidityPeriodUtils.fromLocalInputs(values.validFrom, values.validTo),
      name: values.name.trim(),
      code: values.code.trim().toUpperCase(),
    };

    if (!fields.name || !fields.code) {
      this.formError.set('Kuvvet komutanlığı adı ve kodu zorunludur.');
      return;
    }

    const edited = this.editingUnit();
    const req: Observable<ForcesCommandDefMutationResponse> = edited
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
        this.editingUnit.set(null);
        this.successMessage.set(edited ? 'Kuvvet komutanlığı güncellendi.' : 'Yeni kuvvet komutanlığı oluşturuldu.');
        this.loadUnits();
      },
      error: (error: HttpErrorResponse) => this.formError.set(this.errorText(error))
    });
  }

  /** İlişki korumasını API'de de uygulayan aktiflik değişikliği için onay ister. */
  public confirmStatusChange(unit: ForcesCommandDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId()) return;
    this.confirmations.confirm({
      header: 'Kuvvet Komutanlığı Durumu',
      message: unit.name + ' kaydını ' +
        (unit.isActive ? 'pasifleştirmek' : 'aktifleştirmek') + ' istiyor musunuz?',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet',
      rejectLabel: 'Vazgeç',
      accept: () => this.runRowAction(unit,
        this.service.changeStatus({
          id: unit.id, version: unit.version, isActive: !unit.isActive
        }),
        'Kuvvet komutanlığı durumu değiştirildi.')
    });
  }

  /** Bağlı birlik varsa backend silmeyi reddeder. */
  public confirmDelete(unit: ForcesCommandDefItem): void {
    if (!this.canDelete() || this.actionLoadingId()) return;
    this.confirmations.confirm({
      header: 'Kuvvet Komutanlığını Sil',
      message: unit.name + ' silinsin mi? Bağlı birlik varsa silinemez.',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.runRowAction(unit,
        this.service.delete(unit.id, unit.version), 'Kuvvet komutanlığı silindi.')
    });
  }

  private runRowAction(unit: ForcesCommandDefItem, request: Observable<unknown>,
                       success: string): void {
    if (this.actionLoadingId()) return;
    this.actionLoadingId.set(unit.id);
    request.pipe(finalize(() => this.actionLoadingId.set(null))).subscribe({
      next: () => { this.successMessage.set(success); this.loadUnits(); },
      error: (error: HttpErrorResponse) => this.errorMessage.set(this.errorText(error))
    });
  }

  public openAudit(unit: ForcesCommandDefItem): void {
    if (this.canViewAudit()) this.auditUnit.set(unit);
  }

  /** Merkezi ASP.NET ProblemDetails yanıtını kullanıcı dostu hale getirir. */
  private errorText(error: HttpErrorResponse): string {
    const detail = error.error?.detail;
    if (typeof detail === 'string' && detail.trim()) return detail;
    if (error.status === 409)
      return 'Kayıt başka bir işlemde değişmiş veya bağlı kayıtları olabilir. Listeyi yenileyiniz.';
    if (error.status === 403) return 'Bu işlem için yetkiniz bulunmuyor.';
    if (error.status === 404) return 'Kuvvet komutanlığı bulunamadı. Listeyi yenileyiniz.';
    return 'İşlem tamamlanamadı. Lütfen tekrar deneyiniz.';
  }
}
