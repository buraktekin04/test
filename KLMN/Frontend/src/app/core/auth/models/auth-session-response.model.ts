import { AuthUser } from './auth-user.model';

/**
 * Login/refresh response modelidir.
 * Refresh token bu modelde bulunmaz.
 */
export interface AuthSessionResponse {
  accessToken: string;
  accessTokenExpiresAt: string;
  user: AuthUser;
}
