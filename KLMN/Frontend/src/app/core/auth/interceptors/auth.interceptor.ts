import { inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  HttpContextToken,
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest
} from '@angular/common/http';
import {
  catchError,
  switchMap,
  throwError
} from 'rxjs';

import { API_BASE_URL } from '../../config/api.tokens';
import { AuthService } from '../services/auth.service';

// auth_retry bilgisini işlem sırasında sonraki kontrol adımları için hazırlar.
const AUTH_RETRY =
  new HttpContextToken<boolean>(() => false);

/**
 * API requestlerine JWT ekler ve 401 durumunda tek-flight refresh uygular.
 *
 * Refresh başarısız olduğunda route değiştirmez.
 */
export const authInterceptor: HttpInterceptorFn =
  (request, next) => {
    // Login, refresh, logout ve mevcut kullanıcı işlemlerini sağlayan servistir.
    const authService = inject(AuthService);
    /** Mesai kısıtı sonucu kullanıcının login ekranına gönderilmesini sağlar. */
    const router = inject(Router);
    // Environment'dan enjekte edilen doğrudan HTTPS API temel adresidir.
    const apiBaseUrl = inject(API_BASE_URL);

    if (!request.url.startsWith(apiBaseUrl)) {
      return next(request);
    }

    // API isteklerine Bearer olarak eklenecek, kalıcı depoya yazılmayan JWT'dir.
    const accessToken = authService.getAccessToken();

    // authenticated request bilgisini işlem sırasında sonraki kontrol adımları için hazırlar.
    let authenticatedRequest =
      addAuthentication(request, accessToken);

    return next(authenticatedRequest)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          /*
           * Mesai bitmişse mevcut JWT ile yapılan istekler 403 döner.
           * Bu durum normal permission 403'ünden farklıdır:
           * bellek oturumu kapatılır, uyarı login ekranında gösterilir.
           */
          if (error.status === 403 &&
              error.error?.title === 'Mesai dışı erişim engellendi.') {
            authService.handleOutsideWorkingHours(
              error.error?.detail ??
              'Mesai saatleri dışında uygulamaya erişim izniniz bulunmuyor.'
            );
            void router.navigate(['/login']);
            return throwError(() => error);
          }

          if (
            error.status !== 401 ||
            shouldSkipAutomaticRefresh(request.url) ||
            request.context.get(AUTH_RETRY)
          ) {
            return throwError(() => error);
          }

          // current access token bilgisini işlem sırasında sonraki kontrol adımları için hazırlar.
          const currentAccessToken =
            authService.getAccessToken();

          if (
            currentAccessToken &&
            accessToken &&
            currentAccessToken !== accessToken
          ) {
            authenticatedRequest =
              addAuthentication(
                request.clone({
                  context:
                    request.context.set(
                      AUTH_RETRY,
                      true
                    )
                }),
                currentAccessToken
              );

            return next(authenticatedRequest);
          }

          return authService.refresh()
            .pipe(
              switchMap(session => {
                // retry request bilgisini işlem sırasında sonraki kontrol adımları için hazırlar.
                const retryRequest =
                  addAuthentication(
                    request.clone({
                      context:
                        request.context.set(
                          AUTH_RETRY,
                          true
                        )
                    }),
                    session.accessToken
                  );

                return next(retryRequest);
              }),

              catchError(refreshError => {
                authService.clearSession();

                return throwError(
                  () => refreshError
                );
              })
            );
        })
      );
  };

function addAuthentication(
  request: HttpRequest<unknown>,
  accessToken: string | null
): HttpRequest<unknown> {
  if (!accessToken) {
    return request.clone({
      withCredentials: true
    });
  }

  return request.clone({
    withCredentials: true,
    setHeaders: {
      Authorization: `Bearer ${accessToken}`
    }
  });
}

function shouldSkipAutomaticRefresh(
  url: string
): boolean {
  return [
    '/auth/login',
    '/auth/refresh',
    '/auth/forgot-password',
    '/auth/reset-password'
  ]
  .some(path => url.includes(path));
}
