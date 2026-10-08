/**
 * Reset password request modelidir.
 */
export interface ResetPasswordRequest {
  /** E-posta ile alınan tek kullanımlık parola sıfırlama tokenıdır. */
  token: string;
  /** Hashlenip kaydedilecek yeni paroladır. */
  newPassword: string;
  /** Yeni parolanın doğru tekrar girildiğini kontrol etmek için kullanılır. */
  confirmPassword: string;
}
