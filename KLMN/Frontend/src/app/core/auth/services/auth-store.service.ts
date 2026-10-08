import { computed, Injectable, signal } from '@angular/core';
import { AuthSessionResponse } from '../models/auth-session-response.model';
import { AuthState } from '../models/auth-state.model';
import { AuthUser } from '../models/auth-user.model';

/**
 * Authentication state'ini memory içerisinde yönetir.
 */
@Injectable({ providedIn: 'root' })
export class AuthStoreService {
  /** state durumunu veya bağımlılığını component içerisinde yönetir. */
  private readonly _state = signal<AuthState>({
    isInitialized: false,
    isAuthenticated: false,
    accessToken: null,
    accessTokenExpiresAt: null,
    user: null
  });

  /** Store içindeki güncel oturum durumunu temsil eden değişkendir. */
  public readonly state = this._state.asReadonly();
  /** Uygulama açılışında session kontrolünün tamamlandığını gösteren signal'dır. */
  public readonly isInitialized = computed(() => this._state().isInitialized);
  /** Kullanıcının doğrulanmış oturum sahibi olduğunu izleyen salt okunur signal'dır. */
  public readonly isAuthenticated = computed(() => this._state().isAuthenticated);
  /** API isteklerinde Authorization Bearer olarak kullanılan bellekteki JWT'dir. */
  public readonly accessToken = computed(() => this._state().accessToken);
  /** Oturum sahibine ait profil ve role/permission verileridir. */
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
