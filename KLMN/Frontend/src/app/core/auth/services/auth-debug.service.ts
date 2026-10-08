import { isDevMode, Injectable } from '@angular/core';
import { AuthSessionResponse } from '../models/auth-session-response.model';

/**
 * Authentication debug çıktılarında token, parola ve diğer gizli
 * değerleri hiçbir koşulda konsola yazmaz.
 */
@Injectable({ providedIn: 'root' })
export class AuthDebugService {
  /** Geliştirme ortamında tokenı loglamadan oturum süresi ve kullanıcı özetini kaydeder. */
  public logSession(source: string, response: AuthSessionResponse): void {
    if (!isDevMode()) {
      return;
    }

    console.info('[KLMN AUTH]', source, {
      accessTokenExpiresAt: response.accessTokenExpiresAt,
      userId: response.user.id,
      roleCount: response.user.roles.length,
      permissionCount: response.user.permissions.length
    });
  }
}
