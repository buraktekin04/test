import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideZoneChangeDetection
} from '@angular/core';
import {
  provideHttpClient,
  withInterceptors
} from '@angular/common/http';
import {
  provideAnimationsAsync
} from '@angular/platform-browser/animations/async';
import { provideRouter } from '@angular/router';

import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';
import { firstValueFrom } from 'rxjs';

import { environment } from '../environments/environment';
import { routes } from './app.routes';
import { API_BASE_URL } from './core/config/api.tokens';
import { authInterceptor } from './core/auth/interceptors/auth.interceptor';
import { AuthService } from './core/auth/services/auth.service';

/**
 * KLMN global Angular configuration'ıdır.
 */
export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({
      eventCoalescing: true
    }),

    provideRouter(routes),

    provideHttpClient(
      withInterceptors([
        authInterceptor
      ])
    ),

    provideAnimationsAsync(),

    providePrimeNG({
      theme: {
        preset: Aura
      }
    }),

    {
      provide: API_BASE_URL,
      useValue: environment.apiBaseUrl
    },

    provideAppInitializer(
      () => {
        const authService = inject(AuthService);

        return firstValueFrom(
          authService.initializeSession()
        );
      }
    )
  ]
};
