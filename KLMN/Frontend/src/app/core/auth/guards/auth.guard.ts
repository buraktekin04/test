import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Authenticated route guard.
 */
export const authGuard: CanActivateFn =
  (_route, state) => {
    // Login, refresh, logout ve mevcut kullanıcı işlemlerini sağlayan servistir.
    const authService = inject(AuthService);
    // Giriş, çıkış ve yetki kontrolü sonrası ekran yönlendirmelerini yapar.
    const router = inject(Router);

    if (authService.isAuthenticated()) {
      return true;
    }

    return router.createUrlTree(
      ['/login'],
      {
        queryParams: {
          returnUrl: state.url
        }
      }
    );
  };
