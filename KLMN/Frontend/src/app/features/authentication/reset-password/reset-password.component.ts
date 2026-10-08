import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';
import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';

import { AuthService } from '../../../core/auth/services/auth.service';

/**
 * Reset password ekranıdır.
 */
@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    PasswordModule,
    ButtonModule,
    CardModule,
    MessageModule
  ],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss'
})
export class ResetPasswordComponent {
  /** Reactive Forms alanlarını tip güvenli şekilde oluşturan Angular servisidir. */
  private readonly formBuilder = inject(FormBuilder);
  /** Giriş, parola sıfırlama ve mevcut oturum işlemlerini yöneten auth servisidir. */
  private readonly authService = inject(AuthService);
  /** URL parametrelerini ve geri dönüş yönlendirmesini sağlayan etkin route'dur. */
  private readonly activatedRoute = inject(ActivatedRoute);
  /** Başarılı işlem veya oturum kaybı sonrası ekran geçişlerini gerçekleştirir. */
  private readonly router = inject(Router);

  /** Sıfırlama isteğinin e-posta bağlantısında taşınan tek kullanımlık tokenıdır. */
  public readonly token =
    this.activatedRoute.snapshot.queryParamMap.get('token');

  /** HTTP isteği devam ederken formun tekrar gönderilmesini engelleyen signal'dır. */
  public readonly loading = signal(false);
  /** Backend ProblemDetails veya doğrulama hatasını kullanıcıya gösteren mesaj signal'ıdır. */
  public readonly errorMessage = signal<string | null>(null);

  /** Alanları ve validation kurallarını içeren reaktif form grubudur. */
  public readonly form =
    this.formBuilder.nonNullable.group(
      {
        newPassword: ['', [Validators.required]],
        confirmPassword: ['', [Validators.required]]
      },
      {
        validators: passwordsMatchValidator()
      }
    );

  /** Form alanları doğrulandıktan sonra API isteğini çalıştırır ve sonucu kullanıcıya bildirir. */
  public submit(): void {
    this.errorMessage.set(null);

    if (!this.token) {
      this.errorMessage.set(
        'Parola sıfırlama bağlantısı geçersizdir.'
      );
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);

    // value değerini form veya HTTP iş akışının sonraki kontrollerinde kullanır.
    const value = this.form.getRawValue();

    this.authService
      .resetPassword({
        token: this.token,
        newPassword: value.newPassword,
        confirmPassword: value.confirmPassword
      })
      .pipe(
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: () => {
          void this.router.navigate(
            ['/login'],
            {
              queryParams: {
                passwordReset: true
              }
            }
          );
        },

        error: (error: HttpErrorResponse) => {
          console.error(
            '[KLMN AUTH] Reset password error:',
            error
          );

          this.errorMessage.set(
            error.error?.detail ??
            'Parola sıfırlama bağlantısı geçersiz veya süresi dolmuş olabilir.'
          );
        }
      });
  }
}

function passwordsMatchValidator(): ValidatorFn {
  return (
    control: AbstractControl
  ): ValidationErrors | null => {
    // Kullanıcının belirlemek istediği yeni paroladır.
    const newPassword =
      control.get('newPassword')?.value;

    // Yeni parola ile aynı girilmesi gereken doğrulama alanıdır.
    const confirmPassword =
      control.get('confirmPassword')?.value;

    if (!newPassword || !confirmPassword) {
      return null;
    }

    return newPassword === confirmPassword
      ? null
      : { passwordMismatch: true };
  };
}
