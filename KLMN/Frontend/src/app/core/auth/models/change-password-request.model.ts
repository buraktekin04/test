/**
 * Change password request modelidir.
 */
export interface ChangePasswordRequest {
  /** Parola güncellemesinden önce doğrulanacak mevcut paroladır. */
  currentPassword: string;
  /** Hashlenip kaydedilecek yeni paroladır. */
  newPassword: string;
  /** Yeni parolanın doğru tekrar girildiğini kontrol etmek için kullanılır. */
  confirmPassword: string;
}
