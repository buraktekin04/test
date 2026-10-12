import { Component, computed, inject, OnInit, signal } from '@angular/core';
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
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { PermissionCodes } from '../../../core/permissions/constants/permission-codes';
import { PermissionService } from '../../../core/permissions/services/permission.service';
import { UserAgreementPdf } from '../models/user-agreement-pdf.model';
import { CreateUserRequest } from '../models/create-user-request.model';
import { UpdateUserRequest } from '../models/update-user-request.model';
import { UserListItem } from '../models/user-list-item.model';
import { UserMutationResponse } from '../models/user-mutation-response.model';
import { UserService } from '../services/user.service';

/**
 * Tanımlamalar > Kullanıcı Tanımları ekranının listeleme, sorgulama,
 * ekleme, düzenleme, aktifleştirme/pasifleştirme ve silme akışlarını yönetir.
 */
@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    DatePipe,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    CheckboxModule,
    PaginatorModule,
    MessageModule,
    DialogModule,
    ConfirmDialogModule
  ],
  providers: [ConfirmationService],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit {
  /** Kullanıcı endpoint'lerini çalıştıran tip güvenli API servisidir. */
  private readonly userService = inject(UserService);

  /** Reactive form kontrollerini oluşturan Angular form servisidir. */
  private readonly formBuilder = inject(FormBuilder);

  /** İstemcide buton ve aksiyonları gösteren yetki servisidir. */
  private readonly permissionService = inject(PermissionService);

  /** Silme ve aktifleştirme gibi kritik işlemlerin onay penceresidir. */
  private readonly confirmationService = inject(ConfirmationService);

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

  /** Seçili kullanıcıya ait PDF belge bilgileridir. */
  public readonly agreementPdfs = signal<UserAgreementPdf[]>([]);
  /** PDF ekleme veya silme işleminin yükleme durumudur. */
  public readonly pdfBusy = signal(false);
  /** Kullanıcının seçtiği ancak henüz yüklenmemiş PDF dosyasıdır. */
  public readonly selectedPdf = signal<File | null>(null);
  /** PDF API işleminin hata açıklamasıdır. */
  public readonly pdfError = signal<string | null>(null);

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
   * Kullanıcı temel alanlarının formudur.
   * Rol/permission yönetimi ayrı işlemdir; edit sırasında parola değişmez.
   */
  public readonly userForm = this.formBuilder.nonNullable.group({
    userName: ['', [Validators.required, Validators.maxLength(100)]],
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    phoneNumber: ['', [Validators.maxLength(30)]],
    clockLimit: [true],
    password: ['']
  });

  /** Sayfa açıldığında ilk liste sorgusunu çalıştırır. */
  public ngOnInit(): void {
    this.loadUsers();
  }

  /** Güncel filtrelerle API'den sayfalanmış kullanıcı listesini yükler. */
  public loadUsers(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    // Formdan okunan arama ve aktiflik filtreleridir.
    const filters = this.filterForm.getRawValue();

    this.userService.getUsers({
      search: filters.search,
      includeInactive: filters.includeInactive,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    })
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

  /** Arama ve pasif kullanıcı dahil etme filtrelerini temizler. */
  public clearFilters(): void {
    this.filterForm.reset({ search: '', includeInactive: false });
    this.pageNumber.set(1);
    this.loadUsers();
  }

  /** PrimeNG paginator olayını bir tabanlı API sayfasına dönüştürür. */
  public onPageChange(event: PaginatorState): void {
    const rows = event.rows ?? this.pageSize();
    const first = event.first ?? 0;

    this.pageSize.set(rows);
    this.pageNumber.set(Math.floor(first / rows) + 1);
    this.loadUsers();
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
      clockLimit: true,
      password: ''
    });
    this.formError.set(null);
    this.agreementPdfs.set([]);
    this.selectedPdf.set(null);
    this.pdfError.set(null);
    this.dialogVisible.set(true);
  }

  /** Düzenlemeden önce kaydı yeniden yükleyerek en yeni xmin değerini alır. */
  public openEditDialog(user: UserListItem): void {
    // Sadece belge görme yetkisi olan kullanıcı düzenleme yapmadan belgeleri açabilir.
    if (!this.canUpdate() && !this.canViewDocuments()) return;

    this.actionLoadingId.set(user.id);
    this.formError.set(null);

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
            clockLimit: detail.clockLimit,
            password: ''
          });
          this.dialogVisible.set(true);
          this.selectedPdf.set(null);
          this.pdfError.set(null);
          if (this.canViewDocuments()) this.loadAgreementPdfs(detail.id);
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
    // Salt belge görüntüleme izni kullanıcı profilinin değiştirilmesine izin vermez.
    if (this.selectedUser() && !this.canUpdate()) return;

    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      this.formError.set('Zorunlu alanları ve e-posta biçimini kontrol ediniz.');
      return;
    }

    const value = this.userForm.getRawValue();
    const selected = this.selectedUser();

    if (!selected && !this.isValidPassword(value.password)) {
      this.formError.set(
        'Parola en az 8 karakter, bir büyük harf, bir küçük harf ve bir rakam içermelidir.'
      );
      return;
    }

    const fields = {
      userName: value.userName.trim(),
      firstName: value.firstName.trim(),
      lastName: value.lastName.trim(),
      email: value.email.trim(),
      phoneNumber: value.phoneNumber.trim() || null,
      clockLimit: value.clockLimit
    };

    // Form aynı alanları kullanır, fakat backend'in create ve update
    // sözleşmelerindeki parola ve concurrency özellikleri farklıdır.
    let operation: Observable<UserMutationResponse>;

    if (selected) {
      const request: UpdateUserRequest = {
        ...fields,
        id: selected.id,
        version: selected.version
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
        this.successMessage.set(
          selected ? 'Kullanıcı bilgileri güncellendi.' : 'Kullanıcı başarıyla oluşturuldu.'
        );
        this.loadUsers();
      },
      error: (error: HttpErrorResponse) =>
        this.formError.set(this.getErrorMessage(error))
    });
  }

  /** Seçili kullanıcıya ait PDF metadata listesini sorgular. */
  public loadAgreementPdfs(userId: string): void {
    this.pdfError.set(null);
    this.userService.getAgreementPdfs(userId).subscribe({
      next: documents => this.agreementPdfs.set(documents),
      error: (error: HttpErrorResponse) =>
        this.pdfError.set(this.getErrorMessage(error))
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
    const user = this.selectedUser();
    const file = this.selectedPdf();
    if (!user || !file || !this.canManageDocuments() || this.pdfBusy()) return;

    this.pdfBusy.set(true);
    this.pdfError.set(null);
    this.userService.uploadAgreementPdf(user.id, file)
      .pipe(finalize(() => this.pdfBusy.set(false)))
      .subscribe({
        next: () => {
          this.selectedPdf.set(null);
          this.loadAgreementPdfs(user.id);
          this.successMessage.set('PDF dokümanı yüklendi.');
        },
        error: (error: HttpErrorResponse) =>
          this.pdfError.set(this.getErrorMessage(error))
      });
  }

  /** PDF'yi kimlik doğrulaması olan API'den Blob olarak indirtir. */
  public downloadPdf(pdf: UserAgreementPdf): void {
    const user = this.selectedUser();
    if (!user) return;

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
      error: (error: HttpErrorResponse) =>
        this.pdfError.set(this.getErrorMessage(error))
    });
  }

  /** PDF kaydını onay alarak siler ve arayüz listesini günceller. */
  public confirmDeletePdf(pdf: UserAgreementPdf): void {
    const user = this.selectedUser();
    if (!user || !this.canManageDocuments()) return;

    this.confirmationService.confirm({
      header: 'PDF Dokümanını Sil',
      message: `${pdf.fileName} dosyasını silmek istiyor musunuz?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil',
      rejectLabel: 'Vazgeç',
      accept: () => {
        this.pdfBusy.set(true);
        this.userService.deleteAgreementPdf(user.id, pdf.id)
          .pipe(finalize(() => this.pdfBusy.set(false)))
          .subscribe({
            next: () => {
              this.loadAgreementPdfs(user.id);
              this.successMessage.set('PDF dokümanı silindi.');
            },
            error: (error: HttpErrorResponse) =>
              this.pdfError.set(this.getErrorMessage(error))
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
