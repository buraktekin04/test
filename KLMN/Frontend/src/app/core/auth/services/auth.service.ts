import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  catchError,
  finalize,
  map,
  Observable,
  of,
  shareReplay,
  tap
} from 'rxjs';

import { API_BASE_URL } from '../../config/api.tokens';
import { AuthSessionResponse } from '../models/auth-session-response.model';
import { AuthUser } from '../models/auth-user.model';
import { ChangePasswordRequest } from '../models/change-password-request.model';
import { ForgotPasswordRequest } from '../models/forgot-password-request.model';
import { LoginRequest } from '../models/login-request.model';
import { ResetPasswordRequest } from '../models/reset-password-request.model';
import { AuthDebugService } from './auth-debug.service';
import { AuthStoreService } from './auth-store.service';

/**
 * KLMN authentication işlemlerinin merkezi servisidir.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);
  private readonly store = inject(AuthStoreService);
  private readonly debugService = inject(AuthDebugService);

  private refreshInFlight$: Observable<AuthSessionResponse> | null = null;

  public readonly isAuthenticated = this.store.isAuthenticated;
  public readonly isInitialized = this.store.isInitialized;
  public readonly currentUser = this.store.user;

  public login(request: LoginRequest): Observable<AuthSessionResponse> {
    return this.http.post<AuthSessionResponse>(
      `${this.apiBaseUrl}/auth/login`,
      request,
      { withCredentials: true }
    )
    .pipe(
      tap(response => {
        this.store.setSession(response);
        this.debugService.logSession('Login successful', response);
      })
    );
  }

  public refresh(): Observable<AuthSessionResponse> {
    if (this.refreshInFlight$) {
      return this.refreshInFlight$;
    }

    const request$ = this.http.post<AuthSessionResponse>(
      `${this.apiBaseUrl}/auth/refresh`,
      {},
      { withCredentials: true }
    )
    .pipe(
      tap(response => {
        this.store.setSession(response);
        this.debugService.logSession('Token refreshed', response);
      }),
      finalize(() => {
        this.refreshInFlight$ = null;
      }),
      shareReplay({
        bufferSize: 1,
        refCount: false
      })
    );

    this.refreshInFlight$ = request$;
    return request$;
  }

  public initializeSession(): Observable<void> {
    if (this.store.isInitialized()) {
      return of(void 0);
    }

    return this.refresh()
      .pipe(
        map(() => void 0),
        catchError(() => {
          this.store.clearSession();
          return of(void 0);
        })
      );
  }

  public me(): Observable<AuthUser> {
    return this.http.get<AuthUser>(
      `${this.apiBaseUrl}/auth/me`,
      { withCredentials: true }
    )
    .pipe(
      tap(user => {
        this.store.setUser(user);
      })
    );
  }

  public forgotPassword(
    request: ForgotPasswordRequest
  ): Observable<void> {
    return this.http.post<void>(
      `${this.apiBaseUrl}/auth/forgot-password`,
      request
    );
  }

  public resetPassword(
    request: ResetPasswordRequest
  ): Observable<void> {
    return this.http.post<void>(
      `${this.apiBaseUrl}/auth/reset-password`,
      request
    )
    .pipe(
      tap(() => this.store.clearSession())
    );
  }

  /**
   * Session yalnızca backend başarılı olduğunda temizlenir.
   */
  public changePassword(
    request: ChangePasswordRequest
  ): Observable<void> {
    return this.http.post<void>(
      `${this.apiBaseUrl}/auth/change-password`,
      request,
      { withCredentials: true }
    )
    .pipe(
      tap(() => this.store.clearSession())
    );
  }

  public logout(): Observable<void> {
    return this.http.post<void>(
      `${this.apiBaseUrl}/auth/logout`,
      {},
      { withCredentials: true }
    )
    .pipe(
      finalize(() => this.store.clearSession())
    );
  }

  public logoutAll(): Observable<void> {
    return this.http.post<void>(
      `${this.apiBaseUrl}/auth/logout-all`,
      {},
      { withCredentials: true }
    )
    .pipe(
      finalize(() => this.store.clearSession())
    );
  }

  public getAccessToken(): string | null {
    return this.store.getAccessToken();
  }

  public clearSession(): void {
    this.store.clearSession();
  }

  public debugCurrentSession(): void {
    const state = this.store.state();
    console.info('[KLMN AUTH] Oturum durumu:', {
      isInitialized: state.isInitialized,
      isAuthenticated: state.isAuthenticated,
      accessTokenExpiresAt: state.accessTokenExpiresAt,
      userId: state.user?.id ?? null
    });
  }
}

