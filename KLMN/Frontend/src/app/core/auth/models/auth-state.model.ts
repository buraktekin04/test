import { AuthUser } from './auth-user.model';

/**
 * Angular authentication state modelidir.
 */
export interface AuthState {
  /** Oturumun ilk yüklenme sırasında doğrulanmış olup olmadığını bildirir. */
  isInitialized: boolean;
  /** Kullanıcıya ait geçerli session bilgilerinin bulunup bulunmadığını bildirir. */
  isAuthenticated: boolean;
  /** API isteklerine Bearer olarak eklenecek, kalıcı depoya yazılmayan JWT'dir. */
  accessToken: string | null;
  /** JWT'nin yenilenmesi için kullanılan son geçerlilik tarihidir. */
  accessTokenExpiresAt: string | null;
  /** Oturum sahibinin güncel profil ve kullanıcı arayüzü izin bilgilerini içerir. */
  user: AuthUser | null;
}
