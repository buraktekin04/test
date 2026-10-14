import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin, Observable } from 'rxjs';

import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { PermissionCodes } from '../../../core/permissions/constants/permission-codes';
import { PermissionService } from '../../../core/permissions/services/permission.service';
import {
  RoleFields, RoleItem, RoleMutationResponse,
  RolePermissionOption, RoleQuery
} from '../models/role.model';
import { RoleService } from '../services/role.service';
import { AuditHistoryComponent } from '../../../shared/audit-history/audit-history.component';

/** Rol Tanımları: kompakt liste, CRUD ve rol-permission yönetimi ekranıdır. */
@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [
    DatePipe, FormsModule, ReactiveFormsModule, ButtonModule, CheckboxModule,
    ConfirmDialogModule, DialogModule, InputTextModule,
    MessageModule, TableModule, TagModule, AuditHistoryComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './role-list.component.html',
  styleUrl: './role-list.component.scss'
})
export class RoleListComponent implements OnInit {
  /** Rol API isteklerini gerçekleştiren tip güvenli servistir. */
  private readonly roleService = inject(RoleService);
  /** Form yapılandırma servisidir. */
  private readonly fb = inject(FormBuilder);
  /** Ekran butonları için güncel kullanıcı izinlerini değerlendirir. */
  private readonly permissionService = inject(PermissionService);
  /** Kritik işlemlerden önce PrimeNG onay kutusunu gösterir. */
  private readonly confirmation = inject(ConfirmationService);

  /** Seçili rolün işlem geçmişinin açıldığı ortak pencere durumu. */
  public readonly auditRole = signal<RoleItem | null>(null);

  public readonly canViewAudit = computed(() =>
    this.permissionService.has(PermissionCodes.AuditLogs.View)
  );

  public openRoleAudit(role: RoleItem): void {
    if (this.canViewAudit()) this.auditRole.set(role);
  }

  /** Kullanıcının rol tanımlama yetkileridir; backend aynı izinleri ayrıca doğrular. */
  public readonly canCreate = computed(() => this.permissionService.has(PermissionCodes.Roles.Create));
  public readonly canUpdate = computed(() => this.permissionService.has(PermissionCodes.Roles.Update));
  public readonly canDelete = computed(() => this.permissionService.has(PermissionCodes.Roles.Delete));
  public readonly canManagePermissions = computed(
    () => this.permissionService.has(PermissionCodes.Roles.ManagePermissions)
  );

  /** Mevcut sayfanın kullanıcıdan bağımsız rol listesi. */
  public readonly roles = signal<RoleItem[]>([]);
  /** Sunucu tarafında filtrelenmiş toplam rol adedi. */
  public readonly totalCount = signal(0);
  /** Bir tabanlı sayfa numarası. */
  public readonly sortField = signal('name');
  public readonly sortOrder = signal(1);

  /** Sunucu sıralaması değiştiğinde ilk sayfadan başlar. */
  public onSort(event: TableLazyLoadEvent): void {
    const field = typeof event.sortField === 'string' ? event.sortField : 'name';
    const order = event.sortOrder === -1 ? -1 : 1;
    if (field === this.sortField() && order === this.sortOrder()) return;
    this.sortField.set(field);
    this.sortOrder.set(order);
    this.pageNumber.set(1);
    this.loadRoles();
  }

  public readonly pageNumber = signal(1);
  /** Sayfa başına görüntülenen rol adedi. */
  public readonly pageSize = signal(20);
  /** Liste sorgusu süresince aktif yükleme durumu. */
  public readonly loading = signal(false);
  /** Tekrarlanan satır işlemlerini önleyen rol anahtarı. */
  public readonly actionLoadingId = signal<string | null>(null);
  /** Excel hazırlama göstergesi. */
  public readonly exporting = signal(false);

  /** Ekrana özel başarı ve hata bildirimleri. */
  public readonly successMessage = signal<string | null>(null);
  public readonly errorMessage = signal<string | null>(null);
  /** Arama ve filtre paneli state'leri. */
  public readonly columnFiltersExpanded = signal(false);

  /** Kullanıcı listesiyle aynı Reactive Form arama yaklaşımıdır. */
  public readonly filterForm = this.fb.nonNullable.group({
    search: '',
    includeInactive: false
  });
  /** Aktiflik ve sistem rolü filtreleri sunucu sorgusuna eklenir. */
  public readonly columnFilterForm = this.fb.group({
    isActive: this.fb.control<boolean | null>(null),
    isSystemRole: this.fb.control<boolean | null>(null)
  });

  /** Rol oluşturma/düzenleme modaline ait durum. */
  public readonly dialogVisible = signal(false);
  /** Düzenlenen rol; null ise yeni rol açılmıştır. */
  public readonly editingRole = signal<RoleItem | null>(null);
  /** Form doğrulama ve işlem hatası. */
  public readonly formError = signal<string | null>(null);
  /** Rol kaydetme göstergesi. */
  public readonly saving = signal(false);
  /** Ad, kod ve açıklama dışında alanları değiştirmeyen rol formudur. */
  public readonly roleForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    code: ['', [
      Validators.required, Validators.maxLength(100),
      Validators.pattern(/^[A-Za-z0-9_]+$/)
    ]],
    description: ['', [Validators.maxLength(500)]]
  });

  /** Kullanıcı listesiyle aynı tek satırlık sayfalama hesaplarıdır. */
  public readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalCount() / this.pageSize()))
  );
  public readonly rangeStart = computed(() =>
    this.totalCount() === 0 ? 0 : (this.pageNumber() - 1) * this.pageSize() + 1
  );
  public readonly rangeEnd = computed(() =>
    Math.min(this.totalCount(), this.pageNumber() * this.pageSize())
  );

  /** Listeye bitişik açılan permission yönetim panelinin durumudur. */
  public readonly permissionsVisible = signal(false);
  /** İzinleri görüntülenen veya düzenlenen rolün güncel DTO'sudur. */
  public readonly permissionsRole = signal<RoleItem | null>(null);
  /** Yetki kataloğunun tamamıdır. */
  public readonly permissionOptions = signal<RolePermissionOption[]>([]);
  /** Şu anda seçilen permission GUID kümesidir. */
  public readonly selectedPermissionIds = signal<Set<string>>(new Set<string>());
  /** Modül listesindeki arama metnidir. */
  public readonly permissionSearch = signal('');
  /** Yalnızca atanmış izinlerin gösterilmesini sağlayan görünüm filtresidir. */
  public readonly onlySelectedPermissions = signal(false);
  /** Yetki modüllerinin açılıp kapanmasını yöneten reaktif kümedir. */
  public readonly expandedPermissionModules = signal<Set<string>>(new Set<string>());
  /** İzin paneli ilk açıldığında veya başarılı kayıtta saklanan baz kümedir. */
  private readonly originalPermissionIds = signal<Set<string>>(new Set<string>());
  /** Değişiklik yapıldıysa kaydetme alanında belirtilir. */
  public readonly permissionsDirty = computed(() => {
    const current = this.selectedPermissionIds();
    const initial = this.originalPermissionIds();
    return current.size !== initial.size ||
      [...current].some(id => !initial.has(id));
  });
  /** Panel açılırken rol ve yetki listesinin yükleme durumudur. */
  public readonly permissionsLoading = signal(false);
  /** Permission kayıt isteği durumu. */
  public readonly permissionsSaving = signal(false);
  /** Panelde gösterilen hata mesajıdır. */
  public readonly permissionsError = signal<string | null>(null);
  /** Eski sorguların panel yeniden açılınca durumu değiştirmesini engeller. */
  private permissionRequestVersion = 0;

  /** Permission modülleri için kullanıcı dostu adlar. */
  private readonly moduleLabels: Record<string, string> = {
    Users: 'Kullanıcı İşlemleri',
    Roles: 'Rol Yönetimi',
    MilitaryUnits: 'Birlik',
    ForcesCommands: 'Kuvvet Komutanlıkları',
    Reports: 'Raporlar',
    Investigations: 'İncelemeler'
  };

  /**
   * Yetkileri modüllere ayırır. Arama ve yalnızca seçili görünümü
   * listeyi filtreler; modül sayacı her zaman gerçek toplamı gösterir.
   */
  public readonly permissionGroups = computed(() => {
    const search = this.permissionSearch().trim().toLocaleLowerCase('tr-TR');
    const selectedOnly = this.onlySelectedPermissions();
    const checked = this.selectedPermissionIds();
    const modules = new Map<string, RolePermissionOption[]>();

    for (const option of this.permissionOptions()) {
      const moduleLabel = this.moduleLabels[option.module] ?? option.module;
      const searchable = (option.name + ' ' + option.code + ' ' +
        option.module + ' ' + moduleLabel).toLocaleLowerCase('tr-TR');

      if (search && !searchable.includes(search)) continue;
      if (selectedOnly && !checked.has(option.id)) continue;

      const options = modules.get(option.module) ?? [];
      options.push(option);
      modules.set(option.module, options);
    }

    return [...modules.entries()].map(([module, options]) => {
      const allModuleOptions = this.permissionOptions()
        .filter(option => option.module === module);

      return {
        module,
        label: this.moduleLabels[module] ?? module,
        options,
        selectedCount: allModuleOptions.filter(option => checked.has(option.id)).length,
        totalCount: allModuleOptions.length
      };
    });
  });

  /** Filtreleme açıkken eşleşen modüller her zaman genişletilmiş görünür. */
  public isPermissionGroupExpanded(module: string): boolean {
    return !!this.permissionSearch().trim() || this.onlySelectedPermissions() ||
      this.expandedPermissionModules().has(module);
  }

  /** Bir modülün açılıp kapanmasını aynı anda diğerlerinden bağımsız yönetir. */
  public togglePermissionGroup(module: string): void {
    const next = new Set(this.expandedPermissionModules());
    if (next.has(module)) next.delete(module);
    else next.add(module);
    this.expandedPermissionModules.set(next);
  }

  /** Tüm yetki modüllerini birden açar veya kapatır. */
  public toggleAllPermissionGroups(): void {
    const modules = [...new Set(this.permissionOptions().map(x => x.module))];
    const allExpanded = modules.every(module => this.expandedPermissionModules().has(module));
    this.expandedPermissionModules.set(new Set(allExpanded ? [] : modules));
  }

  /** Modül gruplarının tamamının açık olup olmadığını hesaplar. */
  public readonly allPermissionGroupsExpanded = computed(() => {
    const modules = [...new Set(this.permissionOptions().map(x => x.module))];
    return modules.length > 0 &&
      modules.every(module => this.expandedPermissionModules().has(module));
  });

  /** Yalnızca seçilmiş yetkilerin gösterilmesini açar veya kapatır. */
  public setOnlySelectedPermissions(checked: boolean): void {
    this.onlySelectedPermissions.set(checked);
  }

  /** Kısa süreli ve manuel kapatılabilir ekran bildirimlerini yönetir. */
  public constructor() {
    effect(onCleanup => {
      if (!this.successMessage()) return;
      const timer = setTimeout(() => this.successMessage.set(null), 10_000);
      onCleanup(() => clearTimeout(timer));
    });
    effect(onCleanup => {
      if (!this.errorMessage()) return;
      const timer = setTimeout(() => this.errorMessage.set(null), 10_000);
      onCleanup(() => clearTimeout(timer));
    });
  }

  /** Ekran ilk açıldığında sunucudan rol listesini getirir. */
  public ngOnInit(): void { this.loadRoles(); }

  /** Filtre değerlerini sunucunun GetRolesQuery modeliyle eşler. */
  private buildQuery(): RoleQuery {
    const base = this.filterForm.getRawValue();
    const filters = this.columnFilterForm.getRawValue();
    return {
      search: base.search,
      includeInactive: base.includeInactive || filters.isActive === false,
      sortField: this.sortField(),
      sortOrder: this.sortOrder(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      isActive: filters.isActive,
      isSystemRole: filters.isSystemRole
    };
  }

  /** Eşzamanlı silme veya güncellemeler sonrası listeyi yeniler. */
  private listRequestVersion = 0;

  public loadRoles(): void {
    const version = ++this.listRequestVersion;
    this.loading.set(true);
    this.errorMessage.set(null);
    this.roleService.list(this.buildQuery())
      .pipe(finalize(() => { if (version === this.listRequestVersion) this.loading.set(false); }))
      .subscribe({
        next: data => {
        if (version !== this.listRequestVersion) return;
          this.roles.set([...data.items]);
          this.totalCount.set(data.totalCount);
        },
        error: (error: HttpErrorResponse) => {
        if (version !== this.listRequestVersion) return;
          this.roles.set([]);
          this.totalCount.set(0);
          this.errorMessage.set(this.getErrorMessage(error));
        }
      });
  }

  /** Üst arama formunu uygular. */
  public search(): void { this.pageNumber.set(1); this.loadRoles(); }
  /** Aktif/sistem rolü filtre alanlarının görünürlüğünü değiştirir. */
  public toggleColumnFilters(): void {
    this.columnFiltersExpanded.update(value => !value);
  }
  /** Kolon filtrelerini uygular. */
  public applyColumnFilters(): void { this.pageNumber.set(1); this.loadRoles(); }
  /** Arama ve kolon filtrelerini temizler. */
  public clearFilters(): void {
    this.filterForm.reset({ search: '', includeInactive: false });
    this.columnFilterForm.reset({ isActive: null, isSystemRole: null });
    this.pageNumber.set(1);
    this.loadRoles();
  }

  /** Sayfalama kontrolünü sayfa sınırlarıyla uygular. */
  public goToPage(page: number): void {
    if (this.loading() || page === this.pageNumber() || page < 1 || page > this.totalPages())
      return;
    this.pageNumber.set(page);
    this.loadRoles();
  }

  /** Sayfa boyutunu doğrular ve ilk sayfaya geri döner. */
  public onPageSizeChange(event: Event): void {
    const size = Number((event.target as HTMLSelectElement).value);
    if (![10, 20, 50, 100].includes(size) || size === this.pageSize()) return;
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.loadRoles();
  }

  /** Sunucuda Excel oluşturarak filtrelenmiş tüm rol kayıtlarını indirir. */
  public exportExcel(): void {
    if (this.exporting()) return;
    this.exporting.set(true);
    this.roleService.export(this.buildQuery())
      .pipe(finalize(() => this.exporting.set(false)))
      .subscribe({
        next: file => {
          const url = URL.createObjectURL(file);
          const anchor = document.createElement('a');
          anchor.href = url;
          anchor.download = 'Rol_Tanimlari_' + new Date().toISOString().slice(0, 10) + '.xlsx';
          anchor.click();
          setTimeout(() => URL.revokeObjectURL(url), 1000);
          this.successMessage.set('Rol listesi Excel dosyasına aktarıldı.');
        },
        error: (error: HttpErrorResponse) =>
          this.errorMessage.set(this.getErrorMessage(error))
      });
  }

  /** Yeni rol modalini boş alanlarla gösterir. */
  public openCreateDialog(): void {
    if (!this.canCreate()) return;
    this.editingRole.set(null);
    this.roleForm.reset({ name: '', code: '', description: '' });
    this.formError.set(null);
    this.dialogVisible.set(true);
  }

  /** Düzenlenecek rolü güncel xmin versiyonuyla sunucudan yükler. */
  public openEditDialog(role: RoleItem): void {
    if (!this.canUpdate() || role.isSystemRole || this.actionLoadingId()) return;
    this.actionLoadingId.set(role.id);
    this.roleService.getById(role.id)
      .pipe(finalize(() => this.actionLoadingId.set(null)))
      .subscribe({
        next: latest => {
          if (latest.isSystemRole) return;
          this.editingRole.set(latest);
          this.roleForm.reset({
            name: latest.name, code: latest.code,
            description: latest.description ?? ''
          });
          this.formError.set(null);
          this.dialogVisible.set(true);
        },
        error: (error: HttpErrorResponse) =>
          this.errorMessage.set(this.getErrorMessage(error))
      });
  }

  /** CREATE veya UPDATE sözleşmesini işlevleri bozmadan ortak formdan gönderir. */
  public saveRole(): void {
    if (this.saving() || (this.editingRole() && !this.canUpdate())) return;
    if (this.roleForm.invalid) {
      this.roleForm.markAllAsTouched();
      this.formError.set('Rol adı ve kodunu kontrol edin. Kod harf, rakam ve alt çizgi içermelidir.');
      return;
    }

    const values = this.roleForm.getRawValue();
    const fields: RoleFields = {
      name: values.name.trim(),
      code: values.code.trim().toUpperCase(),
      description: values.description.trim() || null
    };
    if (!fields.name || !fields.code) {
      this.formError.set('Rol adı ve rol kodu zorunludur.');
      return;
    }

    const role = this.editingRole();
    const action: Observable<RoleMutationResponse> = role
      ? this.roleService.update({ ...fields, id: role.id, version: role.version })
      : this.roleService.create(fields);

    this.formError.set(null);
    this.saving.set(true);
    action.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.dialogVisible.set(false);
        this.editingRole.set(null);
        this.successMessage.set(role ? 'Rol güncellendi.' : 'Yeni rol oluşturuldu.');
        this.loadRoles();
      },
      error: (error: HttpErrorResponse) =>
        this.formError.set(this.getErrorMessage(error))
    });
  }

  /** Sistem dışı rolün aktifliğini değiştirmeden önce onay ister. */
  public confirmStatusChange(role: RoleItem): void {
    if (!this.canUpdate() || role.isSystemRole) return;
    this.confirmation.confirm({
      header: 'Rol Durumunu Değiştir',
      message: role.name + ' rolünü ' +
        (role.isActive ? 'pasifleştirmek' : 'aktifleştirmek') + ' istiyor musunuz?',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet',
      rejectLabel: 'Vazgeç',
      accept: () => this.runRowAction(role, this.roleService.changeStatus({
        id: role.id, version: role.version, isActive: !role.isActive
      }), 'Rol durumu güncellendi.')
    });
  }

  /** Ataması olan veya sistem rolünü silmeye backend de izin vermez. */
  public confirmDelete(role: RoleItem): void {
    if (!this.canDelete() || role.isSystemRole) return;
    this.confirmation.confirm({
      header: 'Rolü Sil',
      message: role.name + ' rolü silinsin mi? Bu işlem soft delete uygular.',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.runRowAction(
        role, this.roleService.delete(role.id, role.version), 'Rol silindi.'
      )
    });
  }

  /** Tek rol üzerinde çalışan işlemi ve genel hata/başarı bildirimini yönetir. */
  private runRowAction(role: RoleItem, request: Observable<unknown>, message: string): void {
    if (this.actionLoadingId()) return;
    this.actionLoadingId.set(role.id);
    request.pipe(finalize(() => this.actionLoadingId.set(null))).subscribe({
      next: () => { this.successMessage.set(message); this.loadRoles(); },
      error: (error: HttpErrorResponse) =>
        this.errorMessage.set(this.getErrorMessage(error))
    });
  }

  /** Role ait izin panelini açar; kaydedilmemiş değişiklikler varsa onay ister. */
  public openPermissions(role: RoleItem): void {
    if (!this.canManagePermissions() || this.permissionsSaving()) return;

    if (this.permissionsVisible() && this.permissionsDirty()) {
      this.confirmation.confirm({
        header: 'Kaydedilmemiş Yetkiler',
        message: 'Değişiklikler kaydedilmedi. Başka rolü açmak istiyor musunuz?',
        icon: 'pi pi-exclamation-triangle',
        acceptLabel: 'Kaydetmeden Devam Et',
        rejectLabel: 'Düzenlemeye Dön',
        accept: () => this.loadPermissionPanel(role)
      });
      return;
    }

    this.loadPermissionPanel(role);
  }

  /** Seçilen rolün güncel yetki kataloğunu eşzamanlı okur. */
  private loadPermissionPanel(role: RoleItem): void {
    const requestVersion = ++this.permissionRequestVersion;
    this.permissionsRole.set(role);
    this.permissionsVisible.set(true);
    this.permissionsLoading.set(true);
    this.permissionsError.set(null);
    this.permissionSearch.set('');
    this.onlySelectedPermissions.set(false);
    this.permissionOptions.set([]);
    this.selectedPermissionIds.set(new Set<string>());
    this.originalPermissionIds.set(new Set<string>());
    this.expandedPermissionModules.set(new Set<string>());

    forkJoin({
      role: this.roleService.getById(role.id),
      options: this.roleService.permissionOptions()
    }).pipe(finalize(() => {
      if (requestVersion === this.permissionRequestVersion)
        this.permissionsLoading.set(false);
    })).subscribe({
      next: ({ role: latest, options }) => {
        if (requestVersion !== this.permissionRequestVersion) return;
        this.permissionsRole.set(latest);
        this.permissionOptions.set(options);

        const activeCodes = new Set(latest.permissionCodes);
        const ids = new Set(options.filter(x => activeCodes.has(x.code)).map(x => x.id));
        this.selectedPermissionIds.set(new Set(ids));
        this.originalPermissionIds.set(new Set(ids));

        // Atanmış izinleri olan modüller öncelikle açık gösterilir.
        const selectedModules = options.filter(x => ids.has(x.id)).map(x => x.module);
        const initialModules = selectedModules.length
          ? selectedModules
          : options.length ? [options[0].module] : [];
        this.expandedPermissionModules.set(new Set(initialModules));
      },
      error: (error: HttpErrorResponse) => {
        if (requestVersion === this.permissionRequestVersion)
          this.permissionsError.set(this.getErrorMessage(error));
      }
    });
  }

  /** Paneli kapatmadan önce kaydedilmemiş izinlerin kaybolmasını engeller. */
  public closePermissions(): void {
    if (this.permissionsSaving()) return;

    if (this.permissionsDirty()) {
      this.confirmation.confirm({
        header: 'Kaydedilmemiş Yetkiler',
        message: 'Kaydedilmemiş yetki değişiklikleri silinsin mi?',
        icon: 'pi pi-exclamation-triangle',
        acceptLabel: 'Değişiklikleri Sil',
        rejectLabel: 'Düzenlemeye Dön',
        accept: () => this.closePermissionPanelNow()
      });
      return;
    }

    this.closePermissionPanelNow();
  }

  /** Eski HTTP isteklerini geçersiz kılar ve panel durumunu temizler. */
  private closePermissionPanelNow(): void {
    ++this.permissionRequestVersion;
    this.permissionsVisible.set(false);
    this.permissionsRole.set(null);
    this.permissionsLoading.set(false);
    this.permissionsError.set(null);
    this.permissionOptions.set([]);
    this.selectedPermissionIds.set(new Set<string>());
    this.originalPermissionIds.set(new Set<string>());
    this.permissionSearch.set('');
    this.onlySelectedPermissions.set(false);
    this.expandedPermissionModules.set(new Set<string>());
  }

  /** Permission arama kutusunu günceller. */
  public setPermissionSearch(event: Event): void {
    this.permissionSearch.set((event.target as HTMLInputElement).value);
  }

  /** Her permission satırındaki seçimi yeni Set oluşturarak günceller. */
  public onPermissionToggle(id: string, checked: boolean): void {
    if (!this.canManagePermissions() || this.permissionsRole()?.isSystemRole ||
        this.permissionsSaving() || this.permissionsLoading()) return;
    const next = new Set(this.selectedPermissionIds());
    if (checked) next.add(id);
    else next.delete(id);
    this.selectedPermissionIds.set(next);
  }

  /** Mevcut permission'ın seçili olup olmadığını gösterir. */
  public isPermissionSelected(id: string): boolean {
    return this.selectedPermissionIds().has(id);
  }

  /** Modüldeki tüm permission'ları seçer veya kaldırır. */
  public toggleModule(module: string, select: boolean): void {
    if (!this.canManagePermissions() || this.permissionsRole()?.isSystemRole ||
        this.permissionsSaving() || this.permissionsLoading()) return;
    const next = new Set(this.selectedPermissionIds());
    for (const option of this.permissionOptions()) {
      if (option.module !== module) continue;
      if (select) next.add(option.id);
      else next.delete(option.id);
    }
    this.selectedPermissionIds.set(next);
  }

  /** Kayıtları tek komutla kaydeder; eşzamanlı değişiklikte sunucu 409 döndürür. */
  public savePermissions(): void {
    const role = this.permissionsRole();
    if (!role || !this.canManagePermissions() || role.isSystemRole ||
        this.permissionsLoading() || this.permissionsSaving() ||
        this.permissionsError() || !this.permissionsDirty()) return;

    const selected = [...this.selectedPermissionIds()];
    this.permissionsSaving.set(true);
    this.permissionsError.set(null);

    this.roleService.updatePermissions({
      id: role.id, version: role.version, permissionIds: selected
    }).pipe(finalize(() => this.permissionsSaving.set(false))).subscribe({
      next: result => {
        const activeCodes = this.permissionOptions()
          .filter(x => this.selectedPermissionIds().has(x.id))
          .map(x => x.code);
        this.permissionsRole.set({
          ...role, version: result.version, permissionCodes: activeCodes
        });
        this.originalPermissionIds.set(new Set(this.selectedPermissionIds()));
        this.successMessage.set('Rol yetkileri güncellendi.');
        this.loadRoles();
      },
      error: (error: HttpErrorResponse) =>
        this.permissionsError.set(this.getErrorMessage(error))
    });
  }

  /** Backend ProblemDetails/FluentValidation yanıtlarını kullanıcı mesajına dönüştürür. */
  private getErrorMessage(error: HttpErrorResponse): string {
    const detail = error.error?.detail;
    if (typeof detail === 'string' && detail.trim()) return detail;
    if (error.status === 409)
      return 'Rol başka bir işlemde değiştirilmiş olabilir. Listeyi yenileyiniz.';
    if (error.status === 403)
      return 'Bu işlem için yetkiniz bulunmuyor.';
    if (error.status === 404)
      return 'Rol bulunamadı. Listeyi yenileyiniz.';
    return 'İşlem sırasında hata oluştu. Lütfen tekrar deneyiniz.';
  }
}
