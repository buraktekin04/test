import { isDevMode, Injectable } from '@angular/core';
import { AuthSessionResponse } from '../models/auth-session-response.model';

/**
 * Development ortamında auth debug çıktıları üretir.
 */
@Injectable({ providedIn: 'root' })
export class AuthDebugService {
  public logSession(source: string, response: AuthSessionResponse): void {
    if (!isDevMode()) {
      return;
    }

    console.group(`[MSKBS AUTH] ${source}`);
    console.log('Authentication Response:', response);
    console.log('Access Token Expires At:', response.accessTokenExpiresAt);
    console.log('User:', response.user);
    console.log('Decoded JWT Payload:', this.decodeJwtPayload(response.accessToken));
    console.groupEnd();
  }

  /**
   * JWT payload bölümünü sadece debug amacıyla çözer.
   */
  public decodeJwtPayload(token: string): Record<string, unknown> | null {
    try {
      const parts = token.split('.');

      if (parts.length !== 3) {
        return null;
      }

      const payload = parts[1]
        .replace(/-/g, '+')
        .replace(/_/g, '/');

      const paddedPayload =
        payload.padEnd(
          payload.length + ((4 - payload.length % 4) % 4),
          '='
        );

      const binary = atob(paddedPayload);
      const bytes = Uint8Array.from(
        binary,
        character => character.charCodeAt(0)
      );

      const json = new TextDecoder().decode(bytes);

      return JSON.parse(json) as Record<string, unknown>;
    }
    catch (error) {
      console.error('[MSKBS AUTH] JWT decode edilemedi.', error);
      return null;
    }
  }
}
