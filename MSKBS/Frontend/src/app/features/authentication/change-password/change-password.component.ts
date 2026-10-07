import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';

import { AuthService } from '../../../core/auth/services/auth.service';

/**
 * Change password ekranıdır.
 */
@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    PasswordModule,
    ButtonModule,
    CardModule,
    MessageModule
  ],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.scss'
})
export class ChangePasswordComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  public readonly loading = signal(false);
  public readonly errorMessage = signal<string | null>(null);

  public readonly form =
    this.formBuilder.nonNullable.group(
      {
        currentPassword: ['', [Validators.required]],
        newPassword: ['', [Validators.required]],
        confirmPassword: ['', [Validators.required]]
      },
      {
        validators: passwordsMatchValidator()
      }
    );

  /**
   * Başarısız request'te login'e yönlendirme yapmaz.
   * Sadece success durumunda session sonlandırılarak login'e gider.
   */
  public submit(): void {
    this.errorMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);

    this.authService
      .changePassword(this.form.getRawValue())
      .pipe(
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: () => {
          void this.router.navigate(
            ['/login'],
            {
              queryParams: {
                passwordChanged: true
              }
            }
          );
        },

        error: (error: HttpErrorResponse) => {
          console.error(
            '[MSKBS AUTH] Change password error:',
            error
          );

          if (error.status === 400) {
            this.errorMessage.set(
              error.error?.detail ??
              'Mevcut parola hatalı veya yeni parola geçerli değildir.'
            );
            return;
          }

          if (error.status === 401) {
            this.errorMessage.set(
              error.error?.detail ??
              'Oturum doğrulanamadı. Lütfen isteği tekrar deneyiniz.'
            );
            return;
          }

          if (error.status === 409) {
            this.errorMessage.set(
              error.error?.detail ??
              'İşlem sırasında bir çakışma oluştu.'
            );
            return;
          }

          this.errorMessage.set(
            error.error?.detail ??
            'Parola değiştirilirken beklenmeyen bir hata oluştu.'
          );
        }
      });
  }
}

function passwordsMatchValidator(): ValidatorFn {
  return (
    control: AbstractControl
  ): ValidationErrors | null => {
    const newPassword =
      control.get('newPassword')?.value;

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
