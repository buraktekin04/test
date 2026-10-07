import { inject } from '@angular/core';
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

const AUTH_RETRY =
  new HttpContextToken<boolean>(() => false);

/**
 * API requestlerine JWT ekler ve 401 durumunda tek-flight refresh uygular.
 *
 * Refresh başarısız olduğunda route değiştirmez.
 */
export const authInterceptor: HttpInterceptorFn =
  (request, next) => {
    const authService = inject(AuthService);
    const apiBaseUrl = inject(API_BASE_URL);

    if (!request.url.startsWith(apiBaseUrl)) {
      return next(request);
    }

    const accessToken = authService.getAccessToken();

    let authenticatedRequest =
      addAuthentication(request, accessToken);

    return next(authenticatedRequest)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          if (
            error.status !== 401 ||
            shouldSkipAutomaticRefresh(request.url) ||
            request.context.get(AUTH_RETRY)
          ) {
            return throwError(() => error);
          }

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
