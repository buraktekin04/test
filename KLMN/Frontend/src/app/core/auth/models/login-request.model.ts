/**
 * Kullanıcının kullanıcı adı/e-posta ve parolası ile
 * oturum açma isteğini temsil eder.
 */
export interface LoginRequest {
  /**
   * Kullanıcının kullanıcı adı veya e-posta adresidir.
   */
  identifier: string;

  /**
   * Kullanıcının açık metin parolasıdır.
   * İstemci tarafında kalıcı olarak saklanmaz.
   */
  password: string;

  /**
   * Kullanıcının giriş yaptığı cihazın okunabilir adıdır.
   */
  deviceName?: string | null;
}
