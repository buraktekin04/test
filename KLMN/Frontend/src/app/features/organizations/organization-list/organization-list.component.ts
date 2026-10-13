import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin, Observable } from 'rxjs';

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
  OrganizationParentOption,
  OrganizationUnitFields,
  OrganizationUnitItem,
  OrganizationUnitMutationResponse,
  OrganizationUnitQuery
} from '../models/organization-unit.model';
import { OrganizationService } from '../services/organization.service';

/** Kullanıcıların görev yeri dropdown'ının veri kaynağı olan organizasyon birimlerini yönetir. */
@Component({
  selector: 'app-organization-list',
  standalone: true,
  imports: [
    DatePipe, ReactiveFormsModule, ButtonModule, CheckboxModule,
    ConfirmDialogModule, DialogModule, InputTextModule,
    MessageModule, SelectModule, TableModule, TagModule, AuditHistoryComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './organization-list.component.html',
  styleUrl: './organization-list.component.scss'
})
export class OrganizationListComponent implements OnInit {
  private readonly service = inject(OrganizationService);
  private readonly fb = inject(FormBuilder);
  private readonly permissionService = inject(PermissionService);
  private readonly confirmations = inject(ConfirmationService);

  /** Menü ve satır butonlarındaki UI görünürlük kontrolleridir; API ayrıca korunur. */
  public readonly canCreate = computed(
    () => this.permissionService.has(PermissionCodes.Organizations.Create));
  public readonly canUpdate = computed(
    () => this.permissionService.has(PermissionCodes.Organizations.Update));
  public readonly canDelete = computed(
    () => this.permissionService.has(PermissionCodes.Organizations.Delete));
  public readonly canViewAudit = computed(
    () => this.permissionService.has(PermissionCodes.AuditLogs.View));

  public readonly units = signal<OrganizationUnitItem[]>([]);
  public readonly totalCount = signal(0);
  public readonly pageNumber = signal(1);
  public readonly pageSize = signal(20);
  public readonly loading = signal(false);
  public readonly exporting = signal(false);
  public readonly actionLoadingId = signal<string | null>(null);
  public readonly successMessage = signal<string | null>(null);
  public readonly errorMessage = signal<string | null>(null);
  public readonly columnFiltersExpanded = signal(false);
  public readonly auditUnit = signal<OrganizationUnitItem | null>(null);

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
    rootOnly: this.fb.control<boolean | null>(null)
  });

  public readonly dialogVisible = signal(false);
  public readonly editingUnit = signal<OrganizationUnitItem | null>(null);
  public readonly parentOptions = signal<OrganizationParentOption[]>([]);
  public readonly parentLoading = signal(false);
  public readonly parentError = signal<string | null>(null);
  public readonly formError = signal<string | null>(null);
  public readonly saving = signal(false);

  /** Geçerli entity'deki Code ve Name veri tipi/uzunluklarıyla eşleşen form. */
  public readonly organizationForm = this.fb.group({
    name: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(250)]),
    code: this.fb.nonNullable.control('', [
      Validators.required, Validators.maxLength(100),
      Validators.pattern(/^[A-Za-z0-9_-]+$/)
    ]),
    parentOrganizationUnitId: this.fb.control<string | null>(null)
  });

  /** Üst birim seçiminde kendi kaydı ve tüm alt birimleri gösterilmez. */
  public readonly parentChoices = computed(() => {
    const current = this.editingUnit();
    const all = this.parentOptions();
    const byId = new Map(all.map(unit => [unit.id, unit]));

    const createsCycle = (optionId: string): boolean => {
      if (!current) return false;
      const visited = new Set<string>();
      let id: string | null = optionId;
      while (id !== null) {
        if (id === current.id || visited.has(id)) return true;
        visited.add(id);
        id = byId.get(id)?.parentOrganizationUnitId ?? null;
      }
      return false;
    };

    const options = all
      .filter(option => !createsCycle(option.id))
      .map(option => ({
        id: option.id,
        label: option.name + ' (' + option.code + ')'
      }));

    // Daha önce atanmış bir üst birim pasifleşse bile düzenleme formunda
    // mevcut bağlantı kaybolmaz; yeni üst birim ataması backend'de doğrulanır.
    if (current?.parentOrganizationUnitId &&
        !options.some(x => x.id === current.parentOrganizationUnitId)) {
      options.push({
        id: current.parentOrganizationUnitId,
        label: (current.parentOrganizationUnitName ?? 'Eski üst birim') + ' (mevcut)'
      });
    }

    return options;
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

  private buildQuery(): OrganizationUnitQuery {
    const form = this.filterForm.getRawValue();
    const columns = this.columnFilterForm.getRawValue();
    return {
      search: form.search,
      includeInactive: form.includeInactive || columns.isActive === false,
      isActive: columns.isActive,
      rootOnly: columns.rootOnly,
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
    this.columnFilterForm.reset({ isActive: null, rootOnly: null });
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
          anchor.download = 'Organizasyon_Tanimlari_' +
            new Date().toISOString().slice(0, 10) + '.xlsx';
          anchor.click();
          setTimeout(() => URL.revokeObjectURL(url), 1000);
          this.successMessage.set('Organizasyon listesi Excel olarak indirildi.');
        },
        error: (error: HttpErrorResponse) =>
          this.errorMessage.set(this.errorText(error))
      });
  }

  /** Yeni birim modalinde kök birim veya aktif üst birim seçilebilir. */
  public openCreateDialog(): void {
    if (!this.canCreate() || this.saving()) return;
    ++this.dialogRequestVersion;
    this.editingUnit.set(null);
    this.organizationForm.reset({
      name: '', code: '', parentOrganizationUnitId: null
    });
    this.formError.set(null);
    this.dialogVisible.set(true);
    this.loadParentOptions();
  }

  /** Kullanıcı değişiklikleriyle çakışmamak için son kaydı ve dropdown'ı birlikte getirir. */
  public openEditDialog(unit: OrganizationUnitItem): void {
    if (!this.canUpdate() || this.actionLoadingId() || this.saving()) return;
    const version = ++this.dialogRequestVersion;
    this.actionLoadingId.set(unit.id);
    this.parentLoading.set(true);
    this.formError.set(null);

    forkJoin({
      detail: this.service.getById(unit.id),
      parents: this.service.parentOptions()
    }).pipe(finalize(() => {
      if (version === this.dialogRequestVersion) {
        this.actionLoadingId.set(null);
        this.parentLoading.set(false);
      }
    })).subscribe({
      next: ({ detail, parents }) => {
        if (version !== this.dialogRequestVersion) return;
        this.editingUnit.set(detail);
        this.parentOptions.set(parents);
        this.parentError.set(null);
        this.organizationForm.reset({
          name: detail.name,
          code: detail.code,
          parentOrganizationUnitId: detail.parentOrganizationUnitId
        });
        this.dialogVisible.set(true);
      },
      error: (error: HttpErrorResponse) => {
        if (version !== this.dialogRequestVersion) return;
        this.errorMessage.set(this.errorText(error));
      }
    });
  }

  /** Üst birim listesi yalnızca aktif tanımları içerir. */
  public loadParentOptions(): void {
    this.parentLoading.set(true);
    this.parentError.set(null);
    const version = this.dialogRequestVersion;
    this.service.parentOptions()
      .pipe(finalize(() => {
        if (version === this.dialogRequestVersion) this.parentLoading.set(false);
      }))
      .subscribe({
        next: result => {
          if (version === this.dialogRequestVersion) this.parentOptions.set(result);
        },
        error: (error: HttpErrorResponse) => {
          if (version === this.dialogRequestVersion) this.parentError.set(this.errorText(error));
        }
      });
  }

  /** Modal X veya Vazgeç işleminde eski bekleyen API cevaplarını geçersiz kılar. */
  public onDialogVisibleChange(visible: boolean): void {
    if (!visible && this.saving()) return;
    this.dialogVisible.set(visible);
    if (!visible) ++this.dialogRequestVersion;
  }

  /** Kullanıcı formundan farklı olarak burada yalnızca birim/üst birim bilgisi kaydedilir. */
  public saveUnit(): void {
    if (this.saving() || (this.editingUnit() ? !this.canUpdate() : !this.canCreate()))
      return;

    if (this.organizationForm.invalid) {
      this.organizationForm.markAllAsTouched();
      this.formError.set('Birim adını ve kodunu kontrol ediniz.');
      return;
    }

    const values = this.organizationForm.getRawValue();
    const fields: OrganizationUnitFields = {
      name: values.name.trim(),
      code: values.code.trim().toUpperCase(),
      parentOrganizationUnitId: values.parentOrganizationUnitId
    };

    if (!fields.name || !fields.code) {
      this.formError.set('Birim adı ve kodu zorunludur.');
      return;
    }

    const edited = this.editingUnit();
    const req: Observable<OrganizationUnitMutationResponse> = edited
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
        this.successMessage.set(edited ? 'Organizasyon güncellendi.' : 'Yeni organizasyon oluşturuldu.');
        this.loadUnits();
      },
      error: (error: HttpErrorResponse) => this.formError.set(this.errorText(error))
    });
  }

  /** İlişki korumasını API'de de uygulayan aktiflik değişikliği için onay ister. */
  public confirmStatusChange(unit: OrganizationUnitItem): void {
    if (!this.canUpdate() || this.actionLoadingId()) return;
    this.confirmations.confirm({
      header: 'Organizasyon Durumu',
      message: unit.name + ' birimini ' +
        (unit.isActive ? 'pasifleştirmek' : 'aktifleştirmek') + ' istiyor musunuz?',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet',
      rejectLabel: 'Vazgeç',
      accept: () => this.runRowAction(unit,
        this.service.changeStatus({
          id: unit.id, version: unit.version, isActive: !unit.isActive
        }),
        'Organizasyon durumu değiştirildi.')
    });
  }

  /** Bağlı kullanıcı ya da alt birim bulunursa backend silmeyi reddeder. */
  public confirmDelete(unit: OrganizationUnitItem): void {
    if (!this.canDelete() || this.actionLoadingId()) return;
    this.confirmations.confirm({
      header: 'Organizasyonu Sil',
      message: unit.name + ' silinsin mi? Bağlı kullanıcı veya alt birim varsa silinemez.',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.runRowAction(unit,
        this.service.delete(unit.id, unit.version), 'Organizasyon silindi.')
    });
  }

  private runRowAction(unit: OrganizationUnitItem, request: Observable<unknown>,
                       success: string): void {
    if (this.actionLoadingId()) return;
    this.actionLoadingId.set(unit.id);
    request.pipe(finalize(() => this.actionLoadingId.set(null))).subscribe({
      next: () => { this.successMessage.set(success); this.loadUnits(); },
      error: (error: HttpErrorResponse) => this.errorMessage.set(this.errorText(error))
    });
  }

  public openAudit(unit: OrganizationUnitItem): void {
    if (this.canViewAudit()) this.auditUnit.set(unit);
  }

  /** Merkezi ASP.NET ProblemDetails yanıtını kullanıcı dostu hale getirir. */
  private errorText(error: HttpErrorResponse): string {
    const detail = error.error?.detail;
    if (typeof detail === 'string' && detail.trim()) return detail;
    if (error.status === 409)
      return 'Kayıt başka bir işlemde değişmiş veya bağlı kayıtları olabilir. Listeyi yenileyiniz.';
    if (error.status === 403) return 'Bu işlem için yetkiniz bulunmuyor.';
    if (error.status === 404) return 'Organizasyon bulunamadı. Listeyi yenileyiniz.';
    return 'İşlem tamamlanamadı. Lütfen tekrar deneyiniz.';
  }
}
