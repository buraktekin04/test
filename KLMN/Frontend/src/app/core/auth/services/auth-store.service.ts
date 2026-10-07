import { computed, Injectable, signal } from '@angular/core';
import { AuthSessionResponse } from '../models/auth-session-response.model';
import { AuthState } from '../models/auth-state.model';
import { AuthUser } from '../models/auth-user.model';

/**
 * Authentication state'ini memory içerisinde yönetir.
 */
@Injectable({ providedIn: 'root' })
export class AuthStoreService {
  private readonly _state = signal<AuthState>({
    isInitialized: false,
    isAuthenticated: false,
    accessToken: null,
    accessTokenExpiresAt: null,
    user: null
  });

  public readonly state = this._state.asReadonly();
  public readonly isInitialized = computed(() => this._state().isInitialized);
  public readonly isAuthenticated = computed(() => this._state().isAuthenticated);
  public readonly accessToken = computed(() => this._state().accessToken);
  public readonly user = computed(() => this._state().user);

  /**
   * Başarılı auth response'unu state'e yazar.
   */
  public setSession(session: AuthSessionResponse): void {
    this._state.set({
      isInitialized: true,
      isAuthenticated: true,
      accessToken: session.accessToken,
      accessTokenExpiresAt: session.accessTokenExpiresAt,
      user: session.user
    });
  }

  /**
   * Kullanıcı bilgisini günceller.
   */
  public setUser(user: AuthUser): void {
    this._state.update(current => ({ ...current, user }));
  }

  /**
   * Authentication state'ini temizler.
   */
  public clearSession(): void {
    this._state.set({
      isInitialized: true,
      isAuthenticated: false,
      accessToken: null,
      accessTokenExpiresAt: null,
      user: null
    });
  }

  /**
   * Access token değerini döndürür.
   */
  public getAccessToken(): string | null {
    return this._state().accessToken;
  }
}
