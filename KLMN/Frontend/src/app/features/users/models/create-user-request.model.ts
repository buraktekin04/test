/**
 * Kullanıcı tanımlama ekranından backend CreateUserCommand'e
 * gönderilecek kullanıcı alanlarıdır. İlk parola yalnızca istekte taşınır.
 */
export interface CreateUserRequest {
  /** Benzersiz kullanıcı adı. */
  userName: string;
  /** Kullanıcının adı. */
  firstName: string;
  /** Kullanıcının soyadı. */
  lastName: string;
  /** Benzersiz e-posta adresi. */
  email: string;
  /** Boş olabilen telefon numarası. */
  phoneNumber: string | null;
  /** En az sekiz karakter ve büyük/küçük harf ile rakam içeren parola. */
  password: string;
  /** true: mesai dışı erişim kısıtlıdır; false: mesai dışı izinlidir. */
  clockLimit: boolean;
}
