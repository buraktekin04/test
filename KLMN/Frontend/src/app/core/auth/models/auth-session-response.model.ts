import { AuthUser } from './auth-user.model';

/**
 * Login/refresh response modelidir.
 * Refresh token bu modelde bulunmaz.
 */
export interface AuthSessionResponse {
  /** API isteklerine Bearer olarak eklenecek, kalıcı depoya yazılmayan JWT'dir. */
  accessToken: string;
  /** JWT'nin yenilenmesi için kullanılan son geçerlilik tarihidir. */
  accessTokenExpiresAt: string;
  /** Oturum sahibinin güncel profil ve kullanıcı arayüzü izin bilgilerini içerir. */
  user: AuthUser;
}
