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
  /** Backend'e tip güvenli REST istekleri gönderen Angular HttpClient bağımlılığıdır. */
  private readonly http = inject(HttpClient);
  /** Environment üzerinden gelen ve proxy kullanmadan API'ye yönelen HTTPS temel adresidir. */
  private readonly apiBaseUrl = inject(API_BASE_URL);
  /** Access token ve kullanıcı bilgilerini yalnızca bellekte tutan merkezi store'dur. */
  private readonly store = inject(AuthStoreService);
  /** Gizli token değerini göstermeden güvenli auth durumu kaydeden geliştirme servisidir. */
  private readonly debugService = inject(AuthDebugService);

  /** Çakışan HTTP refresh isteklerini tek çağrıda birleştiren paylaşımlı observable'dır. */
  private refreshInFlight$: Observable<AuthSessionResponse> | null = null;

  /** Kullanıcının doğrulanmış oturum sahibi olduğunu izleyen salt okunur signal'dır. */
  public readonly isAuthenticated = this.store.isAuthenticated;
  /** Uygulama açılışında session kontrolünün tamamlandığını gösteren signal'dır. */
  public readonly isInitialized = this.store.isInitialized;
  /** Oturum sahibinin profil ve etkin izinlerini sağlayan salt okunur signal'dır. */
  public readonly currentUser = this.store.user;

  /** Kullanıcı kimlik bilgilerini API'ye gönderip başarılı sonucu oturuma kaydeder. */
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

  /** HttpOnly cookie ile yeni JWT alır ve paralel yenilemeleri tek istekte birleştirir. */
  public refresh(): Observable<AuthSessionResponse> {
    if (this.refreshInFlight$) {
      return this.refreshInFlight$;
    }

    // Paylaşılan token yenileme HTTP çağrısının RxJS observable'ıdır.
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

  /** Sayfa açılırken HttpOnly cookie varsa oturumu otomatik olarak geri yükler. */
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

  /** Güncel profil, rol ve permission bilgilerini sunucudan yükler. */
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

  /** Kullanıcı varlığını ifşa etmeden parola sıfırlama e-postası ister. */
  public forgotPassword(
    request: ForgotPasswordRequest
  ): Observable<void> {
    return this.http.post<void>(
      `${this.apiBaseUrl}/auth/forgot-password`,
      request
    );
  }

  /** Tek kullanımlık bağlantı ile parolayı günceller ve oturum state'ini sıfırlar. */
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

  /** Mevcut tarayıcının refresh oturumunu sunucuda kapatır. */
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

  /** Bütün cihazların refresh ve access oturumlarını geçersiz kılan endpointi çağırır. */
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

  /** Interceptor'ın ihtiyaç duyduğu bellekteki JWT değerini döndürür. */
  public getAccessToken(): string | null {
    return this.store.getAccessToken();
  }

  /** JWT ve kullanıcı verilerini bellekten temizleyerek oturum durumunu sıfırlar. */
  public clearSession(): void {
    this.store.clearSession();
  }

  /** Erişim tokenı veya parola gibi gizli değerleri göstermeden güncel oturum özetini loglar. */
  public debugCurrentSession(): void {
    // Store içindeki güncel oturum durumunu temsil eden değişkendir.
    const state = this.store.state();
    console.info('[KLMN AUTH] Oturum durumu:', {
      isInitialized: state.isInitialized,
      isAuthenticated: state.isAuthenticated,
      accessTokenExpiresAt: state.accessTokenExpiresAt,
      userId: state.user?.id ?? null
    });
  }
}

