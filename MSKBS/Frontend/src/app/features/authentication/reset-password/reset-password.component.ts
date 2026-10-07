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
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly router = inject(Router);

  public readonly token =
    this.activatedRoute.snapshot.queryParamMap.get('token');

  public readonly loading = signal(false);
  public readonly errorMessage = signal<string | null>(null);

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
            '[MSKBS AUTH] Reset password error:',
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
