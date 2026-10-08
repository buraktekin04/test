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
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);

  public readonly loading = signal(false);
  public readonly errorMessage = signal<string | null>(null);
  public readonly informationMessage = signal<string | null>(null);

  public readonly form =
    this.formBuilder.nonNullable.group({
      identifier: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });

  public constructor() {
    const passwordChanged =
      this.activatedRoute.snapshot.queryParamMap.get('passwordChanged');

    const passwordReset =
      this.activatedRoute.snapshot.queryParamMap.get('passwordReset');

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

  public submit(): void {
    this.errorMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);

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

  private resolveErrorMessage(
    error: HttpErrorResponse
  ): string {
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

  private isSafeReturnUrl(url: string): boolean {
    return url.startsWith('/') && !url.startsWith('//');
  }
}

