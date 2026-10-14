import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
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
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { MultiSelectModule } from 'primeng/multiselect';

import { AuthService } from '../../../core/auth/services/auth.service';
import { UserRoleOption } from '../models/user-role-option.model';
import { ValidityPeriodUtils } from '../../../core/utils/validity-period.utils';
import { PermissionCodes } from '../../../core/permissions/constants/permission-codes';
import { PermissionService } from '../../../core/permissions/services/permission.service';
import { MilitaryUnitDefOption } from '../models/military-unit-def-option.model';
import { UserAgreementPdf } from '../models/user-agreement-pdf.model';
import { CreateUserRequest } from '../models/create-user-request.model';
import { UpdateUserRequest } from '../models/update-user-request.model';
import { UserListItem } from '../models/user-list-item.model';
import { UserQuery } from '../models/user-query.model';
import { UserMutationResponse } from '../models/user-mutation-response.model';
import { UserService } from '../services/user.service';
import { AuditHistoryComponent } from '../../../shared/audit-history/audit-history.component';

/** Dropdown'un kullanıcıya gösterdiği kod + ad etiketli birim seçeneğidir. */
type MilitaryUnitSelectOption = MilitaryUnitDefOption & { label: string };
type UserFormField = 'firstName' | 'lastName' | 'userName' | 'email' | 'phoneNumber'
  | 'militaryUnitDefId' | 'roleIds' | 'password' | 'validFrom' | 'validTo';

/**
 * Tanımlamalar > Kullanıcı Tanımları ekranının listeleme, sorgulama,
 * ekleme, düzenleme, aktifleştirme/pasifleştirme ve silme akışlarını yönetir.
 */
@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    DatePipe,
    SelectModule,
    MultiSelectModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    CheckboxModule,
    MessageModule,
    DialogModule,
    ConfirmDialogModule,
    AuditHistoryComponent
  ],
  providers: [ConfirmationService],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit {
  /** Seçili kullanıcının değişiklik geçmişi dialog içeriğidir. */
  public readonly auditUser = signal<UserListItem | null>(null);
  /** Audit Log görüntüleme yetkisi backend endpoint'inde de denetlenir. */
  public readonly canViewAudit = computed(() =>
    this.permissionService.has(PermissionCodes.AuditLogs.View)
  );

  public openUserAudit(user: UserListItem): void {
    if (this.canViewAudit()) this.auditUser.set(user);
  }

  /** Kullanıcı endpoint'lerini çalıştıran tip güvenli API servisidir. */
  private readonly userService = inject(UserService);
  private readonly authService = inject(AuthService);
  public readonly canAssignRoles = computed(() =>
    this.permissionService.has(PermissionCodes.Users.AssignRole) &&
    this.selectedUser()?.id !== this.authService.currentUser()?.id);
  public readonly roleOptions = signal<(UserRoleOption & { label: string })[]>([]);
  public readonly roleLoading = signal(false);
  public readonly roleLoadError = signal<string | null>(null);
  private roleRequestId = 0;

  public loadRoleOptions(): void {
    const requestId = ++this.roleRequestId;
    this.roleLoadError.set(null);
    this.roleLoading.set(false);
    this.roleOptions.set([]);
    if (!this.canAssignRoles()) return;
    const selected = this.selectedUser();
    this.roleLoading.set(true);
    this.userService.getRoleOptions().subscribe({
      next: roles => {
        if (requestId !== this.roleRequestId) return;
        // Güvenli varsayılan: API yanlışlıkla ADMIN döndürse bile yeni atamaya sunma.
        const assignableRoles = roles.filter(role => role.code.toUpperCase() !== 'ADMIN');
        const availableIds = new Set(assignableRoles.map(role => role.id));
        // Eskiden ADMIN verilmiş kullanıcıda mevcut rol gösterilir; kaldırılabilir,
        // ancak başka bir kullanıcıya yeni ADMIN ataması yapılamaz.
        const existing = (selected?.assignedRoles ?? []).filter(role => !availableIds.has(role.id));
        this.roleOptions.set([
          ...assignableRoles.map(role => ({ ...role, label: `${role.name} (${role.code})` })),
          ...existing.map(role => ({
            ...role,
            label: role.code.toUpperCase() === 'ADMIN'
              ? 'ADMIN (mevcut atama — kaldırılabilir)'
              : `${role.name} (${role.code}) — mevcut atama`
          }))
        ]);
        if (!selected && !this.userForm.controls.roleIds.dirty && this.userForm.controls.roleIds.value.length === 0) {
          const standard = roles.find(role => role.code === 'STANDARD_USER');
          if (standard) this.userForm.controls.roleIds.setValue([standard.id]);
        }
        this.roleLoading.set(false);
      },
      error: () => {
        if (requestId !== this.roleRequestId) return;
        this.roleLoading.set(false);
        this.roleLoadError.set('Rol listesi yüklenemedi. Yeniden deneyiniz.');
      }
    });
  }

  /** Reactive form kontrollerini oluşturan Angular form servisidir. */
  private readonly formBuilder = inject(FormBuilder);

  /** İstemcide buton ve aksiyonları gösteren yetki servisidir. */
  private readonly permissionService = inject(PermissionService);

  /** Silme ve aktifleştirme gibi kritik işlemlerin onay penceresidir. */
  private readonly confirmationService = inject(ConfirmationService);

  /** Formda seçilebilen aktif birlik birimleri ve mevcut pasif atamadır. */
  public readonly militaryUnitOptions = signal<MilitaryUnitSelectOption[]>([]);

  /** Birlik seçenekleri yüklenirken true olan UI göstergesidir. */
  public readonly militaryUnitLoading = signal(false);

  /** Görev yeri kataloğu yüklenemezse form kaydını engelleyen uyarıdır. */
  public readonly militaryUnitLoadError = signal<string | null>(null);

  /** Geçerli sayfanın kullanıcı kayıtlarıdır. */
  public readonly users = signal<UserListItem[]>([]);

  /** Sunucudan dönen filtrelenmiş toplam kayıt sayısıdır. */
  public readonly totalCount = signal(0);

  /** Bir tabanlı sayfalama numarasıdır. */
  public readonly pageNumber = signal(1);

  /** Sayfa başına kullanıcı adedidir. */
  public readonly pageSize = signal(20);

  /** Liste sorgusunun sürmekte olduğunu gösterir. */
  public readonly loading = signal(false);

  /** Hata mesajını API ProblemDetails sonucundan üretir. */
  public readonly errorMessage = signal<string | null>(null);

  /** Son başarılı kullanıcı işlemini özetleyen bilgi mesajıdır. */
  public readonly successMessage = signal<string | null>(null);

  /** Ekle/düzenle penceresinin görünürlük durumudur. */
  public readonly dialogVisible = signal(false);

  /** Düzenlenmek üzere sunucudan yüklenmiş kullanıcı; null ise yeni kayıttır. */
  public readonly selectedUser = signal<UserListItem | null>(null);

  /** Kullanıcı formunun kaydedilmekte olduğunu belirtir. */
  public readonly saving = signal(false);

  /** Modal içindeki validation veya API hata mesajıdır. */
  public readonly formError = signal<string | null>(null);

  /** Kayıt denemesinde yerel alan hatalarını görünür yapar. */
  public readonly attemptedSave = signal(false);
  /** FluentValidation alan hatalarını ilgili inputlarla eşler. */
  public readonly serverFieldErrors = signal<Partial<Record<UserFormField, string>>>({});

  /** İşlemi devam eden kullanıcının ID'sidir; tekrar tıklamayı önler. */
  public readonly actionLoadingId = signal<string | null>(null);

  /** Belge görüntüleme ve indirme yetkisinin UI kontrolüdür. */
  public readonly canViewDocuments = computed(() =>
    this.permissionService.has(PermissionCodes.Users.ViewDocuments)
  );
  /** Belge yükleme ve silme yetkisinin UI kontrolüdür. */
  public readonly canManageDocuments = computed(() =>
    this.permissionService.has(PermissionCodes.Users.ManageDocuments)
  );

  /** Kullanıcı listesiyle bitişik PDF panelinin görünürlük durumudur. */
  public readonly documentsVisible = signal(false);

  /** Listeye bitişik panelde belgeleri yönetilen kullanıcı; düzenlenen kullanıcıdan ayrıdır. */
  public readonly documentsUser = signal<UserListItem | null>(null);

  /** PDF belge listesi sunucudan alınırken yükleme göstergesidir. */
  public readonly pdfLoading = signal(false);

  /** Geç gelen asenkron PDF liste cevaplarının yeni paneli değiştirmesini engeller. */
  private pdfRequestVersion = 0;

  /** Seçili kullanıcıya ait PDF belge bilgileridir. */
  public readonly agreementPdfs = signal<UserAgreementPdf[]>([]);
  /** PDF ekleme veya silme işleminin yükleme durumudur. */
  public readonly pdfBusy = signal(false);
  /** Kullanıcının seçtiği ancak henüz yüklenmemiş PDF dosyasıdır. */
  public readonly selectedPdf = signal<File | null>(null);
  /** PDF API işleminin hata açıklamasıdır. */
  public readonly pdfError = signal<string | null>(null);

  /** Excel için ayrıca sorgulama yetkisi gereklidir. */
  public readonly canExport = computed(() =>
    this.permissionService.has(PermissionCodes.Users.Query));

  /** Yeni kullanıcı butonunu görme iznidir; gerçek kontrol API'dedir. */
  public readonly canCreate = computed(() =>
    this.permissionService.has(PermissionCodes.Users.Create)
  );

  /** Kullanıcı düzenleme yetkisini gösterir. */
  public readonly canUpdate = computed(() =>
    this.permissionService.has(PermissionCodes.Users.Update)
  );

  /** Kullanıcı silme yetkisini gösterir. */
  public readonly canDelete = computed(() =>
    this.permissionService.has(PermissionCodes.Users.Delete)
  );

  /** Aktifleştirme ve pasifleştirme yetkisini gösterir. */
  public readonly canActivate = computed(() =>
    this.permissionService.has(PermissionCodes.Users.Activate)
  );

  /** Filtreleme kriterlerinin tutulduğu tipli Reactive Form'dur. */
  public readonly filterForm = this.formBuilder.nonNullable.group({
    search: [''],
    includeInactive: [false]
  });

  /**
   * Kolon bazlı filtreler tabloda yalnızca görüntülenen 20 kayda değil,
   * EF Core sorgusuyla tüm veritabanına uygulanır.
   */
  public readonly columnFilterForm = this.formBuilder.group({
    fullName: this.formBuilder.nonNullable.control(''),
    userName: this.formBuilder.nonNullable.control(''),
    email: this.formBuilder.nonNullable.control(''),
    militaryUnit: this.formBuilder.nonNullable.control(''),
    role: this.formBuilder.nonNullable.control(''),
    isActive: this.formBuilder.control<boolean | null>(null),
    isLocked: this.formBuilder.control<boolean | null>(null),
    clockLimit: this.formBuilder.control<boolean | null>(null),
    validFromDate: this.formBuilder.nonNullable.control(''),
    validToDate: this.formBuilder.nonNullable.control(''),
    createdDate: this.formBuilder.nonNullable.control('')
  });

  /**
   * Gelişmiş kolon filtreleri tek bir kompakt panelde gösterilir.
   * Tablo başlığının altında ikinci filtre satırı oluşturulmaz.
   */
  public readonly columnFiltersExpanded = signal(false);

  /** Toplam sayfa sayısıdır (boş listede 1). */
  public readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalCount() / this.pageSize()))
  );

  /** Sayfanın gösterdiği ilk kayıt sırasıdır. */
  public readonly rangeStart = computed(() =>
    this.totalCount() === 0 ? 0 : (this.pageNumber() - 1) * this.pageSize() + 1
  );

  /** Sayfanın gösterdiği son kayıt sırasıdır. */
  public readonly rangeEnd = computed(() =>
    Math.min(this.totalCount(), this.pageNumber() * this.pageSize())
  );

  /** XLSX üretimi devam ederken ikinci isteği önleyen göstergedir. */
  public readonly exporting = signal(false);

  /** Üst bildirimler 10 saniye sonra otomatik silinir, çarpıdan kapatılabilir. */
  public constructor() {
    // Her yeniden oluşan bildirim için zamanlayıcı yeniden başlar.
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
    // Yeni veri girildiğinde eski API doğrulama mesajları artık geçerli değildir.
    this.userForm.valueChanges.pipe(takeUntilDestroyed()).subscribe(() => {
      this.formError.set(null);
      this.serverFieldErrors.set({});
    });
  }

  /**
   * Kullanıcı temel alanlarının formudur.
   * Roller atama yetkisiyle düzenlenir; edit sırasında parola değişmez.
   */
  public readonly userForm = this.formBuilder.nonNullable.group({
    userName: ['', [Validators.required, Validators.maxLength(100)]],
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    phoneNumber: ['', [Validators.maxLength(30)]],
    // Mevcut tabloda birlik ataması boş kalabileceği için nullable'dır.
    militaryUnitDefId: this.formBuilder.control<string | null>(null),
    roleIds: this.formBuilder.nonNullable.control<string[]>([]),
    clockLimit: [true],
    isLocked: [false],
    validFrom: [''],
    validTo: [''],
    password: ['']
  });

  /** Sayfa açıldığında ilk liste sorgusunu çalıştırır. */
  public ngOnInit(): void {
    this.loadUsers();
  }

  /**
   * Açılan formun birlik kataloğunu güncel API verisinden doldurur.
   * Önceden atanmış ama pasif hale gelmiş birimi seçili göstermeye devam eder;
   * yeni bir pasif birlik atamasına API izin vermez.
   */
  public loadMilitaryUnitOptions(selectedUser?: UserListItem): void {
    this.militaryUnitLoading.set(true);
    this.militaryUnitLoadError.set(null);
    this.militaryUnitOptions.set([]);

    this.userService.getMilitaryUnitDefOptions()
      .pipe(finalize(() => this.militaryUnitLoading.set(false)))
      .subscribe({
        next: militaryUnits => {
          const options: MilitaryUnitSelectOption[] = militaryUnits.map(item => ({
            ...item,
            label: `${item.code} — ${item.name}`
          }));

          // Kullanıcının önceki ataması sonradan pasifleşmiş olabilir.
          if (selectedUser?.militaryUnitDefId &&
              !options.some(item => item.id === selectedUser.militaryUnitDefId)) {
            options.unshift({
              id: selectedUser.militaryUnitDefId,
              code: selectedUser.militaryUnitDefCode ?? 'PASIF',
              name: selectedUser.militaryUnitDefName ?? 'Önceden atanmış görev yeri',
              connectedTopUnity: null,
              label: `${selectedUser.militaryUnitDefCode ?? 'Eski'} — ${selectedUser.militaryUnitDefName ?? 'Görev Yeri'} (Mevcut atama)`
            });
          }

          this.militaryUnitOptions.set(options);
        },
        error: (error: HttpErrorResponse) => {
          this.militaryUnitLoadError.set(
            'Görev yeri listesi yüklenemedi. Lütfen yeniden deneyin. ' +
            this.getErrorMessage(error)
          );
        }
      });
  }

  /** Kolon filtrelerini ve üst arama alanını aynı sunucu sorgusunda birleştirir. */
  private buildUserQuery(): UserQuery {
    const filters = this.filterForm.getRawValue();
    const columns = this.columnFilterForm.getRawValue();

    return {
      search: filters.search,
      // Pasif filtresi tek başına seçildiğinde de pasif kayıtları DB sorgusuna alır.
      includeInactive: filters.includeInactive || columns.isActive === false,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      fullName: columns.fullName,
      userName: columns.userName,
      email: columns.email,
      militaryUnit: columns.militaryUnit,
      role: columns.role,
      isActive: columns.isActive,
      isLocked: columns.isLocked,
      clockLimit: columns.clockLimit,
      validFromDate: columns.validFromDate,
      validToDate: columns.validToDate,
      createdDate: columns.createdDate
    };
  }

  /** Güncel filtrelerle API'den sayfalanmış kullanıcı listesini yükler. */
  public loadUsers(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.userService.getUsers(this.buildUserQuery())
    .pipe(finalize(() => this.loading.set(false)))
    .subscribe({
      next: response => {
        this.users.set(response.items);
        this.totalCount.set(response.totalCount);
      },
      error: (error: HttpErrorResponse) => {
        this.users.set([]);
        this.totalCount.set(0);
        this.errorMessage.set(this.getErrorMessage(error));
      }
    });
  }

  /** Arama yapıldığında ilk sayfaya dönerek yeni filtreyi uygular. */
  public search(): void {
    this.pageNumber.set(1);
    this.loadUsers();
  }

  /** Ekran-özel gelişmiş filtre bölümünün açık/kapalı durumunu değiştirir. */
  public toggleColumnFilters(): void {
    this.columnFiltersExpanded.update(open => !open);
  }

  /** Kolon filtreleri değiştiğinde sunucudan ilk sayfayı yeniden sorgular. */
  public applyColumnFilters(): void {
    this.pageNumber.set(1);
    this.loadUsers();
  }

  /** Üst arama ve kolon bazlı tüm filtreleri temizler. */
  public clearFilters(): void {
    this.filterForm.reset({ search: '', includeInactive: false });
    this.columnFilterForm.reset({
      fullName: '', userName: '', email: '', militaryUnit: '', role: '',
      isActive: null, isLocked: null, clockLimit: null,
      validFromDate: '', validToDate: '', createdDate: ''
    });
    this.pageNumber.set(1);
    this.loadUsers();
  }

  /**
   * Tablonun yatay scroll'undan bağımsız tek satırlık sayfalama kontrolleridir.
   * Değişmeyen sayfaya gereksiz HTTP sorgusu göndermez.
   */
  public goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.pageNumber() || this.loading()) {
      return;
    }

    this.pageNumber.set(page);
    this.loadUsers();
  }

  /** Sayfa başına kayıt sayısını değiştirdiğinde ilk sayfayı getirir. */
  public onPageSizeChange(event: Event): void {
    const value = Number((event.target as HTMLSelectElement).value);
    if (![10, 20, 50, 100].includes(value) || value === this.pageSize()) {
      return;
    }

    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadUsers();
  }

  /**
   * Tabloda seçili filtrelerin tüm sonuçlarını Excel dosyası olarak indirir.
   * JWT ve kullanıcı okuma yetkisi backend'de denetlenmeye devam eder.
   */
  public exportExcel(): void {
    if (!this.canExport() || this.exporting()) return;

    this.exporting.set(true);
    this.userService.exportUsers(this.buildUserQuery())
      .pipe(finalize(() => this.exporting.set(false)))
      .subscribe({
        next: file => {
          // Blob URL tarayıcıya özel geçici bir indirme konumudur.
          const url = URL.createObjectURL(file);
          const anchor = document.createElement('a');
          anchor.href = url;
          anchor.download = `Kullanici_Tanimlari_${new Date().toISOString().slice(0, 10)}.xlsx`;
          anchor.click();
          setTimeout(() => URL.revokeObjectURL(url), 1000);
          this.successMessage.set('Filtrelenmiş kullanıcı listesi Excel dosyasına aktarıldı.');
        },
        error: () => {
          this.errorMessage.set(
            'Excel hazırlanamadı. 5.000 kayıt sınırını aşmamak için filtrelerinizi daraltabilir veya tekrar deneyebilirsiniz.'
          );
        }
      });
  }

  /** Yeni kullanıcı formunu boş değerlerle açar. */
  public openCreateDialog(): void {
    if (!this.canCreate()) return;

    this.selectedUser.set(null);
    this.userForm.reset({
      userName: '',
      firstName: '',
      lastName: '',
      email: '',
      phoneNumber: '',
      militaryUnitDefId: null,
      roleIds: [],
      clockLimit: true,
      isLocked: false,
      validFrom: '',
      validTo: '',
      password: ''
    });
    this.resetValidation();
    this.dialogVisible.set(true);
    this.loadMilitaryUnitOptions();
    this.loadRoleOptions();
  }

  /** Düzenlemeden önce kaydı yeniden yükleyerek en yeni xmin değerini alır. */
  public openEditDialog(user: UserListItem): void {
    // Profil düzenleme izni olmayan kullanıcı PDF butonundan belgeleri yönetir.
    if (!this.canUpdate()) return;

    this.actionLoadingId.set(user.id);
    this.resetValidation();

    this.userService.getUserById(user.id)
      .pipe(finalize(() => this.actionLoadingId.set(null)))
      .subscribe({
        next: detail => {
          this.selectedUser.set(detail);
          this.userForm.reset({
            userName: detail.userName,
            firstName: detail.firstName,
            lastName: detail.lastName,
            email: detail.email,
            phoneNumber: detail.phoneNumber ?? '',
            militaryUnitDefId: detail.militaryUnitDefId,
            roleIds: detail.assignedRoles.map(role => role.id),
            clockLimit: detail.clockLimit,
            isLocked: detail.isLocked,
            validFrom: ValidityPeriodUtils.toLocalInput(detail.validFrom),
            validTo: ValidityPeriodUtils.toLocalInput(detail.validTo),
            password: ''
          });
          this.dialogVisible.set(true);
          this.loadMilitaryUnitOptions(detail);
          this.loadRoleOptions();
        },
        error: (error: HttpErrorResponse) =>
          this.errorMessage.set(this.getErrorMessage(error))
      });
  }

  /**
   * Create ve Update komutlarının ortak formunu doğrular.
   * Yeni kayıtta parola güvenlik şartları sağlanmalıdır.
   */
  public saveUser(): void {
    if (this.saving()) return;
    if (this.selectedUser() ? !this.canUpdate() : !this.canCreate()) return;
    this.attemptedSave.set(true);
    this.formError.set(null);
    this.serverFieldErrors.set({});

    if (this.canAssignRoles() &&
        (this.roleLoading() || this.roleLoadError() || !this.userForm.controls.roleIds.value.length)) {
      this.formError.set('Rol listesini yükleyip en az bir rol seçiniz.');
      this.focusFormField('userRoleIds');
      return;
    }
    if (this.militaryUnitLoading() || this.militaryUnitLoadError()) {
      this.formError.set('Görev yeri listesi yüklenemedi. Seçenekleri yenileyiniz.');
      this.focusFormField('userMilitaryUnitDefId');
      return;
    }
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      const first = (['firstName', 'lastName', 'userName', 'email', 'phoneNumber'] as const)
        .find(key => this.userForm.controls[key].invalid);
      if (first) this.focusFormField(this.fieldIds[first]);
      return;
    }

    const value = this.userForm.getRawValue();
    const selected = this.selectedUser();
    if (!ValidityPeriodUtils.hasValidRange(value.validFrom, value.validTo)) {
      this.focusFormField('userValidTo');
      return;
    }
    if (!selected && !this.isValidPassword(value.password)) {
      this.focusFormField('newUserPassword');
      return;
    }

    const fields = {
      userName: value.userName.trim(),
      firstName: value.firstName.trim(),
      lastName: value.lastName.trim(),
      email: value.email.trim(),
      phoneNumber: value.phoneNumber.trim() || null,
      militaryUnitDefId: value.militaryUnitDefId,
      roleIds: this.canAssignRoles() ? value.roleIds : undefined,
      clockLimit: value.clockLimit,
      ...ValidityPeriodUtils.fromLocalInputs(value.validFrom, value.validTo)
    };

    // Form aynı alanları kullanır, fakat backend'in create ve update
    // sözleşmelerindeki parola ve concurrency özellikleri farklıdır.
    let operation: Observable<UserMutationResponse>;

    if (selected) {
      const request: UpdateUserRequest = {
        ...fields,
        id: selected.id,
        version: selected.version,
        isLocked: value.isLocked
      };
      operation = this.userService.updateUser(request);
    } else {
      const request: CreateUserRequest = {
        ...fields,
        password: value.password
      };
      operation = this.userService.createUser(request);
    }

    this.formError.set(null);
    this.saving.set(true);

    operation.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.dialogVisible.set(false);
        this.selectedUser.set(null);
        this.userForm.controls.password.setValue('');
        this.resetValidation();
        this.successMessage.set(
          selected ? 'Kullanıcı bilgileri güncellendi.' : 'Kullanıcı başarıyla oluşturuldu.'
        );
        this.loadUsers();
      },
      error: (error: HttpErrorResponse) => this.showSaveError(error)
    });
  }

  private readonly fieldIds: Record<UserFormField, string> = {
    firstName: 'editFirstName', lastName: 'editLastName',
    userName: 'editUserName', email: 'editEmail',
    phoneNumber: 'editPhoneNumber', militaryUnitDefId: 'userMilitaryUnitDefId',
    roleIds: 'userRoleIds', password: 'newUserPassword',
    validFrom: 'userValidFrom', validTo: 'userValidTo'
  };

  private resetValidation(): void {
    this.attemptedSave.set(false);
    this.formError.set(null);
    this.serverFieldErrors.set({});
  }

  /** Zorunlu alan, e-posta ve uzunluk hatalarını girişin hemen altında gösterir. */
  public fieldError(field: UserFormField): string | null {
    const serverMessage = this.serverFieldErrors()[field];
    if (serverMessage) return serverMessage;
    const control = this.userForm.controls[field];
    if (!control.invalid || (!control.touched && !this.attemptedSave())) return null;
    if (control.hasError('required')) return 'Bu alan zorunludur.';
    if (control.hasError('email')) return 'Geçerli bir e-posta adresi giriniz.';
    if (control.hasError('maxlength')) return 'Karakter sınırı aşıldı.';
    return 'Bu alanın değerini kontrol ediniz.';
  }

  public passwordError(): string | null {
    if (this.selectedUser()) return null;
    const serverMessage = this.serverFieldErrors().password;
    if (serverMessage) return serverMessage;
    const control = this.userForm.controls.password;
    if (!this.attemptedSave() && !control.touched) return null;
    if (!control.value) return 'İlk parola zorunludur.';
    return this.isValidPassword(control.value) ? null
      : '8–128 karakter, büyük/küçük harf ve rakam gereklidir.';
  }

  public validityError(): string | null {
    const serverMessage = this.serverFieldErrors().validTo;
    if (serverMessage) return serverMessage;
    const from = this.userForm.controls.validFrom;
    const to = this.userForm.controls.validTo;
    if (!this.attemptedSave() && !from.touched && !to.touched) return null;
    return ValidityPeriodUtils.hasValidRange(from.value, to.value)
      ? null : 'Bitiş tarihi başlangıçtan sonra olmalıdır.';
  }

  public roleError(): string | null {
    if (!this.canAssignRoles()) return null;
    const serverMessage = this.serverFieldErrors().roleIds;
    if (serverMessage) return serverMessage;
    return this.attemptedSave() && !this.roleLoading() &&
      !this.userForm.controls.roleIds.value.length ? 'En az bir rol seçiniz.' : null;
  }

  /** Sorunlu inputu kaydırarak görünür kılar; ilgili alanı odaklar. */
  private focusFormField(id: string): void {
    requestAnimationFrame(() => {
      if (!this.dialogVisible()) return;
      const element = document.getElementById(id);
      if (!element) return;
      element.scrollIntoView({ behavior: 'smooth', block: 'center' });
      if (element instanceof HTMLElement) {
        const input = element.matches('input,textarea,button')
          ? element : element.querySelector<HTMLElement>('input,button,[tabindex]');
        input?.focus({ preventScroll: true });
      }
    });
  }

  /** API alan hataları inline; genel/çakışma hataları Kaydet yanında görünür. */
  private showSaveError(error: HttpErrorResponse): void {
    const errors: unknown = error.error?.errors;
    const mapped: Partial<Record<UserFormField, string>> = {};
    const keys = Object.keys(this.fieldIds) as UserFormField[];
    if (errors && typeof errors === 'object' && !Array.isArray(errors)) {
      for (const [property, messages] of Object.entries(errors)) {
        const field = keys.find(key => key.toLowerCase() === property.split('.').pop()?.toLowerCase());
        if (field && Array.isArray(messages) && typeof messages[0] === 'string')
          mapped[field] = messages[0];
      }
    }
    this.serverFieldErrors.set(mapped);
    const first = keys.find(field => mapped[field]);
    if (first) {
      this.formError.set('İşaretlenen alanları kontrol edip yeniden kaydediniz.');
      this.focusFormField(this.fieldIds[first]);
    } else {
      this.formError.set(this.getErrorMessage(error));
    }
  }

  /** Formdaki geçerlilik tarihinin değerini temizler. */
  public clearValidityDate(field: 'validFrom' | 'validTo'): void {
    this.userForm.controls[field].setValue('');
  }

  /**
   * Satırdaki PDF butonuyla kullanıcı listesinin yanındaki belge panelini açar.
   * Düzenleme formunu ve militaryUnit endpoint'ini çalıştırmaz.
   */
  public openDocumentsPanel(user: UserListItem): void {
    if (!this.canViewDocuments()) return;

    ++this.pdfRequestVersion;
    this.documentsUser.set(user);
    this.agreementPdfs.set([]);
    this.selectedPdf.set(null);
    this.pdfError.set(null);
    this.documentsVisible.set(true);
    this.loadAgreementPdfs(user.id);
  }

  /** Panel kapatma butonunda yalnızca PDF ekranının durumunu temizler. */
  public onDocumentsVisibleChange(visible: boolean): void {
    this.documentsVisible.set(visible);
    if (visible) return;

    ++this.pdfRequestVersion;
    this.documentsUser.set(null);
    this.agreementPdfs.set([]);
    this.selectedPdf.set(null);
    this.pdfError.set(null);
    this.pdfLoading.set(false);
  }

  /**
   * Belge listesini yalnızca panelde seçili kullanıcı için yükler.
   * Panel kapatılıp başka kullanıcıya geçilirse eski isteğin cevabı gösterilmez.
   */
  public loadAgreementPdfs(userId: string): void {
    if (!this.documentsVisible() || this.documentsUser()?.id !== userId) return;

    this.pdfError.set(null);
    this.pdfLoading.set(true);
    const requestVersion = ++this.pdfRequestVersion;

    this.userService.getAgreementPdfs(userId)
      .pipe(finalize(() => {
        if (this.documentsUser()?.id === userId &&
            this.pdfRequestVersion === requestVersion) {
          this.pdfLoading.set(false);
        }
      }))
      .subscribe({
        next: documents => {
          if (this.documentsVisible() &&
              this.documentsUser()?.id === userId &&
              this.pdfRequestVersion === requestVersion) {
            this.agreementPdfs.set(documents);
          }
        },
        error: (error: HttpErrorResponse) => {
          if (this.documentsVisible() &&
              this.documentsUser()?.id === userId &&
              this.pdfRequestVersion === requestVersion) {
            this.pdfError.set(this.getErrorMessage(error));
          }
        }
      });
  }

  /** Dosya seçimini ve PDF/10 MB istemci ön kontrollerini gerçekleştirir. */
  public onPdfSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.item(0) ?? null;
    this.selectedPdf.set(null);
    this.pdfError.set(null);

    if (!file) return;

    if (!file.name.toLowerCase().endsWith('.pdf')) {
      this.pdfError.set('Yalnızca PDF uzantılı dosya yüklenebilir.');
      input.value = '';
      return;
    }

    // Sunucunun ayarındaki boyut sınırı ayrıca kesin olarak doğrulanır.
    if (file.size === 0 || file.size > 10 * 1024 * 1024) {
      this.pdfError.set('PDF boş olmamalı ve 10 MB sınırını aşmamalıdır.');
      input.value = '';
      return;
    }

    this.selectedPdf.set(file);
    // Aynı PDF ikinci kez seçilirse change event'inin tekrar çalışmasını sağlar.
    input.value = '';
  }

  /** Sunucu klasörüne PDF yükledikten sonra doküman listesini yeniler. */
  public uploadPdf(): void {
    const user = this.documentsUser();
    const file = this.selectedPdf();
    if (!user || !this.documentsVisible() || !file || !this.canManageDocuments() || this.pdfBusy()) return;

    this.pdfBusy.set(true);
    this.pdfError.set(null);
    this.userService.uploadAgreementPdf(user.id, file)
      .pipe(finalize(() => this.pdfBusy.set(false)))
      .subscribe({
        next: () => {
          if (this.documentsVisible() && this.documentsUser()?.id === user.id) {
            this.selectedPdf.set(null);
            this.loadAgreementPdfs(user.id);
          }
          this.successMessage.set('PDF dokümanı yüklendi.');
        },
        error: (error: HttpErrorResponse) => {
          if (this.documentsVisible() && this.documentsUser()?.id === user.id) {
            this.pdfError.set(this.getErrorMessage(error));
          }
        }
      });
  }

  /** PDF'yi listede seçili belge paneli kullanıcısı için yetkili API'den indirir. */
  public downloadPdf(pdf: UserAgreementPdf): void {
    const user = this.documentsUser();
    if (!user || !this.documentsVisible() || !this.canViewDocuments()) return;

    this.pdfError.set(null);
    this.userService.downloadAgreementPdf(user.id, pdf.id).subscribe({
      next: data => {
        const objectUrl = URL.createObjectURL(data);
        const anchor = document.createElement('a');
        anchor.href = objectUrl;
        anchor.download = pdf.fileName;
        anchor.click();
        // Download başladığında kullanılan geçici URL sistemden temizlenir.
        setTimeout(() => URL.revokeObjectURL(objectUrl), 1000);
      },
      error: (error: HttpErrorResponse) => {
        if (this.documentsVisible() && this.documentsUser()?.id === user.id) {
          this.pdfError.set(this.getErrorMessage(error));
        }
      }
    });
  }

  /** PDF kaydını listeye bitişik panelde seçili kullanıcı için onay alarak siler. */
  public confirmDeletePdf(pdf: UserAgreementPdf): void {
    const user = this.documentsUser();
    if (!user || !this.canManageDocuments()) return;

    this.confirmationService.confirm({
      header: 'PDF Dokümanını Sil',
      message: `${pdf.fileName} dosyasını silmek istiyor musunuz?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      accept: () => {
        // Onay gecikmesi sırasında kullanıcı/panel değişmişse silme işlemi başlatma.
        if (!this.documentsVisible() || this.documentsUser()?.id !== user.id) return;
        this.pdfBusy.set(true);
        this.userService.deleteAgreementPdf(user.id, pdf.id)
          .pipe(finalize(() => this.pdfBusy.set(false)))
          .subscribe({
            next: () => {
              if (this.documentsVisible() && this.documentsUser()?.id === user.id) {
                this.loadAgreementPdfs(user.id);
              }
              this.successMessage.set('PDF dokümanı silindi.');
            },
            error: (error: HttpErrorResponse) => {
              if (this.documentsVisible() && this.documentsUser()?.id === user.id) {
                this.pdfError.set(this.getErrorMessage(error));
              }
            }
          });
      }
    });
  }

  /** Status değişimini kullanıcının onayı alındıktan sonra gerçekleştirir. */
  public confirmStatusChange(user: UserListItem): void {
    if (!this.canActivate()) return;

    // Pasifleştirme/aktifleştirme işlemi yanlış tıklamayla çalıştırılmaz.
    const action = user.isActive ? 'pasifleştirmek' : 'aktifleştirmek';

    this.confirmationService.confirm({
      header: 'Kullanıcı Durumunu Değiştir',
      message: `${user.fullName} adlı kullanıcıyı ${action} istiyor musunuz?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet',
      rejectLabel: 'Vazgeç',
      accept: () => {
        this.runRowAction(
          user,
          this.userService.changeUserStatus({
            id: user.id,
            version: user.version,
            isActive: !user.isActive
          }),
          'Kullanıcı durumu güncellendi.'
        );
      }
    });
  }

  /** Kullanıcı silme işlemini soft delete öncesinde onaya sunar. */
  public confirmDelete(user: UserListItem): void {
    if (!this.canDelete()) return;

    this.confirmationService.confirm({
      header: 'Kullanıcıyı Sil',
      message: `${user.fullName} adlı kullanıcı silinsin mi? Bu işlem soft delete uygulanarak yapılacaktır.`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.runRowAction(
          user,
          this.userService.deleteUser(user.id, user.version),
          'Kullanıcı silindi.'
        );
      }
    });
  }

  /** Tablo satırında işlem sırasında tekrar tıklamayı engeller ve listeyi yeniler. */
  private runRowAction(
    user: UserListItem,
    operation: Observable<unknown>,
    message: string
  ): void {
    this.actionLoadingId.set(user.id);
    this.errorMessage.set(null);

    operation.pipe(finalize(() => this.actionLoadingId.set(null))).subscribe({
      next: () => {
        this.successMessage.set(message);
        this.loadUsers();
      },
      error: (error: HttpErrorResponse) =>
        this.errorMessage.set(this.getErrorMessage(error))
    });
  }

  /** Yeni kullanıcı parolasının temel güvenlik koşullarını ön tarafta denetler. */
  private isValidPassword(password: string): boolean {
    return password.length >= 8 &&
      password.length <= 128 &&
      /[A-Z]/.test(password) &&
      /[a-z]/.test(password) &&
      /[0-9]/.test(password);
  }

  /** API'den gelen ProblemDetails açıklamasını güvenli kullanıcı mesajına çevirir. */
  private getErrorMessage(error: HttpErrorResponse): string {
    const detail = error.error?.detail;

    if (typeof detail === 'string' && detail.trim()) return detail;

    if (error.status === 409) {
      return 'Kayıt başka bir kullanıcı tarafından değiştirilmiş olabilir. Listeyi yenileyiniz.';
    }

    if (error.status === 403) return 'Bu işlem için yetkiniz bulunmuyor.';
    if (error.status === 404) return 'Kullanıcı bulunamadı, listeyi yenileyiniz.';

    return 'İşlem sırasında hata oluştu. Lütfen tekrar deneyiniz.';
  }
}
