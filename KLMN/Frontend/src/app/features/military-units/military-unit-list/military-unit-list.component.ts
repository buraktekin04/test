import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { ValidityPeriodUtils } from '../../../core/utils/validity-period.utils';
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
  MilitaryUnitParentOption,
  MilitaryUnitDefFields,
  MilitaryUnitDefItem,
  MilitaryUnitDefMutationResponse,
  MilitaryUnitDefQuery
} from '../models/military-unit-def.model';
import { MilitaryUnitService } from '../services/military-unit.service';

/** Kullanıcıların görev yeri dropdown'ının veri kaynağı olan birliklerni yönetir. */
@Component({
  selector: 'app-military-unit-list',
  standalone: true,
  imports: [
    DatePipe, ReactiveFormsModule, ButtonModule, CheckboxModule,
    ConfirmDialogModule, DialogModule, InputTextModule,
    MessageModule, SelectModule, TableModule, TagModule, AuditHistoryComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './military-unit-list.component.html',
  styleUrl: './military-unit-list.component.scss'
})
export class MilitaryUnitListComponent implements OnInit {
  private readonly service = inject(MilitaryUnitService);
  private readonly fb = inject(FormBuilder);
  private readonly permissionService = inject(PermissionService);
  private readonly confirmations = inject(ConfirmationService);

  /** Menü ve satır butonlarındaki UI görünürlük kontrolleridir; API ayrıca korunur. */
  public readonly canCreate = computed(
    () => this.permissionService.has(PermissionCodes.MilitaryUnits.Create));
  public readonly canUpdate = computed(
    () => this.permissionService.has(PermissionCodes.MilitaryUnits.Update));
  public readonly canDelete = computed(
    () => this.permissionService.has(PermissionCodes.MilitaryUnits.Delete));
  public readonly canViewAudit = computed(
    () => this.permissionService.has(PermissionCodes.AuditLogs.View));

  public readonly units = signal<MilitaryUnitDefItem[]>([]);
  public readonly totalCount = signal(0);
  public readonly pageNumber = signal(1);
  public readonly pageSize = signal(20);
  public readonly loading = signal(false);
  public readonly exporting = signal(false);
  public readonly actionLoadingId = signal<string | null>(null);
  public readonly successMessage = signal<string | null>(null);
  public readonly errorMessage = signal<string | null>(null);
  public readonly columnFiltersExpanded = signal(false);
  public readonly auditUnit = signal<MilitaryUnitDefItem | null>(null);

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
  public readonly editingUnit = signal<MilitaryUnitDefItem | null>(null);
  public readonly parentOptions = signal<MilitaryUnitParentOption[]>([]);
  public readonly parentLoading = signal(false);
  public readonly parentError = signal<string | null>(null);
  public readonly formError = signal<string | null>(null);
  public readonly saving = signal(false);

  /** Geçerli entity'deki Code ve Name veri tipi/uzunluklarıyla eşleşen form. */
  public readonly militaryUnitForm = this.fb.group({
    name: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(250)]),
    code: this.fb.nonNullable.control('', [
      Validators.required, Validators.maxLength(100),
      Validators.pattern(/^[A-Za-z0-9_-]+$/)
    ]),
    externalCode: this.fb.nonNullable.control('', Validators.maxLength(100)),
    forcesCommandDef: this.fb.nonNullable.control('', Validators.pattern(/^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/)),
    validFrom: this.fb.nonNullable.control(''),
    validTo: this.fb.nonNullable.control(''),
    connectedTopUnity: this.fb.control<string | null>(null)
  });

  /** Üst birlik seçiminde kendi kaydı ve tüm alt birlikleri gösterilmez. */
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
        id = byId.get(id)?.connectedTopUnity ?? null;
      }
      return false;
    };

    const options = all
      .filter(option => !createsCycle(option.id))
      .map(option => ({
        id: option.id,
        label: option.name + ' (' + option.code + ')'
      }));

    // Daha önce atanmış bir üst birlik pasifleşse bile düzenleme formunda
    // mevcut bağlantı kaybolmaz; yeni üst birlik ataması backend'de doğrulanır.
    if (current?.connectedTopUnity &&
        !options.some(x => x.id === current.connectedTopUnity)) {
      options.push({
        id: current.connectedTopUnity,
        label: (current.connectedTopUnitName ?? 'Eski üst birlik') + ' (mevcut)'
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

  private buildQuery(): MilitaryUnitDefQuery {
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
          anchor.download = 'Birlik_Tanimlari_' +
            new Date().toISOString().slice(0, 10) + '.xlsx';
          anchor.click();
          setTimeout(() => URL.revokeObjectURL(url), 1000);
          this.successMessage.set('Birlik listesi Excel olarak indirildi.');
        },
        error: (error: HttpErrorResponse) =>
          this.errorMessage.set(this.errorText(error))
      });
  }

  /** Yeni birlik modalinde kök birlik veya aktif üst birlik seçilebilir. */
  public openCreateDialog(): void {
    if (!this.canCreate() || this.saving()) return;
    ++this.dialogRequestVersion;
    this.editingUnit.set(null);
    this.militaryUnitForm.reset({
      name: '', code: '', externalCode: '', forcesCommandDef: '', validFrom: '', validTo: '', connectedTopUnity: null
    });
    this.formError.set(null);
    this.dialogVisible.set(true);
    this.loadParentOptions();
  }

  /** Kullanıcı değişiklikleriyle çakışmamak için son kaydı ve dropdown'ı birlikte getirir. */
  public openEditDialog(unit: MilitaryUnitDefItem): void {
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
        this.militaryUnitForm.reset({
          externalCode: detail.externalCode ?? '',
          forcesCommandDef: detail.forcesCommandDef ?? '',
          validFrom: ValidityPeriodUtils.toLocalInput(detail.validFrom),
          validTo: ValidityPeriodUtils.toLocalInput(detail.validTo),
          name: detail.name,
          code: detail.code,
          connectedTopUnity: detail.connectedTopUnity
        });
        this.dialogVisible.set(true);
      },
      error: (error: HttpErrorResponse) => {
        if (version !== this.dialogRequestVersion) return;
        this.errorMessage.set(this.errorText(error));
      }
    });
  }

  /** Üst birlik listesi yalnızca aktif tanımları içerir. */
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

  /** Kullanıcı formundan farklı olarak burada yalnızca birlik/üst birlik bilgisi kaydedilir. */
  public saveUnit(): void {
    if (this.saving() || (this.editingUnit() ? !this.canUpdate() : !this.canCreate()))
      return;

    if (this.militaryUnitForm.invalid) {
      this.militaryUnitForm.markAllAsTouched();
      this.formError.set('Birlik adı, kodu ve kuvvet komutanlığı referansını kontrol ediniz.');
      return;
    }

    const values = this.militaryUnitForm.getRawValue();
    if (!ValidityPeriodUtils.hasValidRange(values.validFrom, values.validTo)) {
      this.formError.set('Bitiş tarihi başlangıç tarihinden sonra olmalıdır.');
      return;
    }
    const fields: MilitaryUnitDefFields = {
      externalCode: values.externalCode.trim() || null,
      forcesCommandDef: values.forcesCommandDef.trim() || null,
      ...ValidityPeriodUtils.fromLocalInputs(values.validFrom, values.validTo),
      name: values.name.trim(),
      code: values.code.trim().toUpperCase(),
      connectedTopUnity: values.connectedTopUnity
    };

    if (!fields.name || !fields.code) {
      this.formError.set('Birlik adı ve kodu zorunludur.');
      return;
    }

    const edited = this.editingUnit();
    const req: Observable<MilitaryUnitDefMutationResponse> = edited
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
        this.successMessage.set(edited ? 'Birlik güncellendi.' : 'Yeni birlik oluşturuldu.');
        this.loadUnits();
      },
      error: (error: HttpErrorResponse) => this.formError.set(this.errorText(error))
    });
  }

  /** İlişki korumasını API'de de uygulayan aktiflik değişikliği için onay ister. */
  public confirmStatusChange(unit: MilitaryUnitDefItem): void {
    if (!this.canUpdate() || this.actionLoadingId()) return;
    this.confirmations.confirm({
      header: 'Birlik Durumu',
      message: unit.name + ' birlikini ' +
        (unit.isActive ? 'pasifleştirmek' : 'aktifleştirmek') + ' istiyor musunuz?',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet',
      rejectLabel: 'Vazgeç',
      accept: () => this.runRowAction(unit,
        this.service.changeStatus({
          id: unit.id, version: unit.version, isActive: !unit.isActive
        }),
        'Birlik durumu değiştirildi.')
    });
  }

  /** Bağlı kullanıcı ya da alt birlik bulunursa backend silmeyi reddeder. */
  public confirmDelete(unit: MilitaryUnitDefItem): void {
    if (!this.canDelete() || this.actionLoadingId()) return;
    this.confirmations.confirm({
      header: 'Birliği Sil',
      message: unit.name + ' silinsin mi? Bağlı kullanıcı veya alt birlik varsa silinemez.',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.runRowAction(unit,
        this.service.delete(unit.id, unit.version), 'Birlik silindi.')
    });
  }

  private runRowAction(unit: MilitaryUnitDefItem, request: Observable<unknown>,
                       success: string): void {
    if (this.actionLoadingId()) return;
    this.actionLoadingId.set(unit.id);
    request.pipe(finalize(() => this.actionLoadingId.set(null))).subscribe({
      next: () => { this.successMessage.set(success); this.loadUnits(); },
      error: (error: HttpErrorResponse) => this.errorMessage.set(this.errorText(error))
    });
  }

  public openAudit(unit: MilitaryUnitDefItem): void {
    if (this.canViewAudit()) this.auditUnit.set(unit);
  }

  /** Merkezi ASP.NET ProblemDetails yanıtını kullanıcı dostu hale getirir. */
  private errorText(error: HttpErrorResponse): string {
    const detail = error.error?.detail;
    if (typeof detail === 'string' && detail.trim()) return detail;
    if (error.status === 409)
      return 'Kayıt başka bir işlemde değişmiş veya bağlı kayıtları olabilir. Listeyi yenileyiniz.';
    if (error.status === 403) return 'Bu işlem için yetkiniz bulunmuyor.';
    if (error.status === 404) return 'Birlik bulunamadı. Listeyi yenileyiniz.';
    return 'İşlem tamamlanamadı. Lütfen tekrar deneyiniz.';
  }
}
