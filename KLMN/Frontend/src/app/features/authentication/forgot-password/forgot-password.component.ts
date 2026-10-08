import { Component, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';

import { AuthService } from '../../../core/auth/services/auth.service';

/**
 * Forgot password ekranıdır.
 */
@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    InputTextModule,
    ButtonModule,
    CardModule,
    MessageModule
  ],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss'
})
export class ForgotPasswordComponent {
  /** Reactive Forms alanlarını tip güvenli şekilde oluşturan Angular servisidir. */
  private readonly formBuilder = inject(FormBuilder);
  /** Giriş, parola sıfırlama ve mevcut oturum işlemlerini yöneten auth servisidir. */
  private readonly authService = inject(AuthService);

  /** HTTP isteği devam ederken formun tekrar gönderilmesini engelleyen signal'dır. */
  public readonly loading = signal(false);
  /** success message durumunu veya bağımlılığını component içerisinde yönetir. */
  public readonly successMessage = signal<string | null>(null);
  /** Backend ProblemDetails veya doğrulama hatasını kullanıcıya gösteren mesaj signal'ıdır. */
  public readonly errorMessage = signal<string | null>(null);

  /** Alanları ve validation kurallarını içeren reaktif form grubudur. */
  public readonly form =
    this.formBuilder.nonNullable.group({
      email: [
        '',
        [
          Validators.required,
          Validators.email
        ]
      ]
    });

  /** Form alanları doğrulandıktan sonra API isteğini çalıştırır ve sonucu kullanıcıya bildirir. */
  public submit(): void {
    this.successMessage.set(null);
    this.errorMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);

    this.authService
      .forgotPassword(this.form.getRawValue())
      .pipe(
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: () => {
          this.successMessage.set(
            'Bu e-posta adresi sistemde kayıtlıysa parola sıfırlama bağlantısı gönderilmiştir.'
          );
        },

        error: (error: HttpErrorResponse) => {
          console.error(
            '[KLMN AUTH] Forgot password error:',
            error
          );

          this.errorMessage.set(
            'Parola sıfırlama talebi sırasında bir hata oluştu.'
          );
        }
      });
  }
}
