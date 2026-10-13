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
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { PermissionCodes } from '../../../core/permissions/constants/permission-codes';
import { PermissionService } from '../../../core/permissions/services/permission.service';
import {
  RoleFields, RoleItem, RoleMutationResponse,
  RolePermissionOption, RoleQuery
} from '../models/role.model';
import { RoleService } from '../services/role.service';

/** Rol Tanımları: kompakt liste, CRUD ve rol-permission yönetimi ekranıdır. */
@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [
    DatePipe, ReactiveFormsModule, ButtonModule, CheckboxModule,
    ConfirmDialogModule, DialogModule, InputTextModule,
    MessageModule, TableModule, TagModule
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
    Organizations: 'Organizasyon',
    Reports: 'Raporlar',
    Investigations: 'İncelemeler'
  };

  /** Gruplu ve filtrelenmiş permission katalog görünümüdür. */
  public readonly permissionGroups = computed(() => {
    const search = this.permissionSearch().trim().toLocaleLowerCase('tr-TR');
    const checked = this.selectedPermissionIds();
    const modules = new Map<string, RolePermissionOption[]>();

    for (const option of this.permissionOptions()) {
      const searchable = (option.name + ' ' + option.code + ' ' + option.module)
        .toLocaleLowerCase('tr-TR');
      if (search && !searchable.includes(search)) continue;
      const existing = modules.get(option.module) ?? [];
      existing.push(option);
      modules.set(option.module, existing);
    }

    return [...modules.entries()].map(([module, options]) => ({
      module,
      label: this.moduleLabels[module] ?? module,
      options,
      selectedCount: options.filter(option => checked.has(option.id)).length
    }));
  });

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
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      isActive: filters.isActive,
      isSystemRole: filters.isSystemRole
    };
  }

  /** Eşzamanlı silme veya güncellemeler sonrası listeyi yeniler. */
  public loadRoles(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.roleService.list(this.buildQuery())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: data => {
          this.roles.set([...data.items]);
          this.totalCount.set(data.totalCount);
        },
        error: (error: HttpErrorResponse) => {
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

  /** Seçilen role ait izinleri listeyle bütünleşik sağ panelde gösterir. */
  public openPermissions(role: RoleItem): void {
    if (!this.canManagePermissions() || this.permissionsSaving()) return;

    const requestVersion = ++this.permissionRequestVersion;
    this.permissionsRole.set(role);
    this.permissionsVisible.set(true);
    this.permissionsLoading.set(true);
    this.permissionsError.set(null);
    this.permissionSearch.set('');
    this.permissionOptions.set([]);
    this.selectedPermissionIds.set(new Set<string>());

    // Güncel rol versiyonu ile aktif permission kataloğu eş zamanlı yüklenir.
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
        this.selectedPermissionIds.set(
          new Set(options.filter(x => activeCodes.has(x.code)).map(x => x.id))
        );
      },
      error: (error: HttpErrorResponse) => {
        if (requestVersion === this.permissionRequestVersion)
          this.permissionsError.set(this.getErrorMessage(error));
      }
    });
  }

  /** Permission panelini kapatır; eski HTTP yanıtlarını artık kabul etmez. */
  public closePermissions(): void {
    if (this.permissionsSaving()) return;
    ++this.permissionRequestVersion;
    this.permissionsVisible.set(false);
    this.permissionsRole.set(null);
    this.permissionsLoading.set(false);
    this.permissionsError.set(null);
    this.permissionOptions.set([]);
    this.selectedPermissionIds.set(new Set<string>());
  }

  /** Permission arama kutusunu günceller. */
  public setPermissionSearch(event: Event): void {
    this.permissionSearch.set((event.target as HTMLInputElement).value);
  }

  /** Her permission satırındaki seçimi yeni Set oluşturarak günceller. */
  public onPermissionToggle(id: string, event: Event): void {
    if (!this.canManagePermissions() || this.permissionsRole()?.isSystemRole) return;
    const checked = (event.target as HTMLInputElement).checked;
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
    if (!this.canManagePermissions() || this.permissionsRole()?.isSystemRole) return;
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
        this.permissionsLoading() || this.permissionsSaving() || this.permissionsError()) return;

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
