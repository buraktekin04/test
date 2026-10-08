import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';
import { finalize } from 'rxjs';

import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';

import { AuthService } from '../../../core/auth/services/auth.service';
import { ProblemDetails } from '../../../core/models/problem-details.model';

/**
 * KLMN kullanıcı giriş ekranıdır.
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    InputTextModule,
    PasswordModule,
    ButtonModule,
    CardModule,
    MessageModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  /** Reactive Forms alanlarını tip güvenli şekilde oluşturan Angular servisidir. */
  private readonly formBuilder = inject(FormBuilder);
  /** Giriş, parola sıfırlama ve mevcut oturum işlemlerini yöneten auth servisidir. */
  private readonly authService = inject(AuthService);
  /** Başarılı işlem veya oturum kaybı sonrası ekran geçişlerini gerçekleştirir. */
  private readonly router = inject(Router);
  /** URL parametrelerini ve geri dönüş yönlendirmesini sağlayan etkin route'dur. */
  private readonly activatedRoute = inject(ActivatedRoute);

  /** HTTP isteği devam ederken formun tekrar gönderilmesini engelleyen signal'dır. */
  public readonly loading = signal(false);
  /** Backend ProblemDetails veya doğrulama hatasını kullanıcıya gösteren mesaj signal'ıdır. */
  public readonly errorMessage = signal<string | null>(null);
  /** Başarılı parola değişimi, sıfırlama veya çıkış mesajını ekrana taşır. */
  public readonly informationMessage = signal<string | null>(null);

  /** Alanları ve validation kurallarını içeren reaktif form grubudur. */
  public readonly form =
    this.formBuilder.nonNullable.group({
      identifier: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });

  /** function Object() { [native code] } */
  public constructor() {
    // Giriş ekranında parola değişikliği sonrası gösterilecek bilginin URL bayrağıdır.
    const passwordChanged =
      this.activatedRoute.snapshot.queryParamMap.get('passwordChanged');

    // Giriş ekranında reset sonrası gösterilecek bilginin URL bayrağıdır.
    const passwordReset =
      this.activatedRoute.snapshot.queryParamMap.get('passwordReset');

    // Giriş ekranında çıkış sonrası gösterilecek bilginin URL bayrağıdır.
    const loggedOut =
      this.activatedRoute.snapshot.queryParamMap.get('loggedOut');

    if (passwordChanged === 'true') {
      this.informationMessage.set(
        'Parolanız başarıyla değiştirildi. Yeni parolanızla giriş yapabilirsiniz.'
      );
      return;
    }

    if (passwordReset === 'true') {
      this.informationMessage.set(
        'Parolanız başarıyla sıfırlandı. Yeni parolanızla giriş yapabilirsiniz.'
      );
      return;
    }

    if (loggedOut === 'true') {
      this.informationMessage.set(
        'Oturumunuz başarıyla kapatıldı.'
      );
    }
  }

  /** Form alanları doğrulandıktan sonra API isteğini çalıştırır ve sonucu kullanıcıya bildirir. */
  public submit(): void {
    this.errorMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);

    // Form alanlarından API sözleşmesine göre oluşturulan istek nesnesidir.
    const request = {
      ...this.form.getRawValue(),
      deviceName: 'Web'
    };

    this.authService.login(request)
      .pipe(
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: response => {
          // Login sonrası dönülmek istenen dahili uygulama yönlendirmesidir.
          const returnUrl =
            this.activatedRoute.snapshot.queryParamMap.get('returnUrl');

          if (
            returnUrl &&
            this.isSafeReturnUrl(returnUrl)
          ) {
            void this.router.navigateByUrl(returnUrl);
            return;
          }

          void this.router.navigate(['/app']);
        },

        error: (error: HttpErrorResponse) => {
          console.error('[KLMN AUTH] Login hatası:', error);
          this.errorMessage.set(
            this.resolveErrorMessage(error)
          );
        }
      });
  }

  /** API durum kodunu ve ProblemDetails alanlarını anlaşılır Türkçe mesaja dönüştürür. */
  private resolveErrorMessage(
    error: HttpErrorResponse
  ): string {
    // Backend'in alan bazlı hata ve mesajlarını içeren ProblemDetails nesnesidir.
    const problem =
      error.error as ProblemDetails | null;

    if (error.status === 423) {
      return problem?.detail ??
        'Hesabınız geçici olarak kilitlenmiştir.';
    }

    if (error.status === 401) {
      return problem?.detail ??
        'Kullanıcı adı/e-posta veya parola hatalı.';
    }

    if (error.status === 400) {
      return problem?.detail ??
        'Gönderilen bilgiler geçerli değildir.';
    }

    if (error.status === 0) {
      return 'API sunucusuna ulaşılamıyor.';
    }

    return problem?.detail ??
      problem?.title ??
      'Giriş sırasında beklenmeyen bir hata oluştu.';
  }

  /** Harici siteye yönlendirmeyi engellemek için yalnızca güvenli dahili URL'leri kabul eder. */
  private isSafeReturnUrl(url: string): boolean {
    return url.startsWith('/') && !url.startsWith('//');
  }
}

