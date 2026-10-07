import { AuthUser } from './auth-user.model';

/**
 * Angular authentication state modelidir.
 */
export interface AuthState {
  isInitialized: boolean;
  isAuthenticated: boolean;
  accessToken: string | null;
  accessTokenExpiresAt: string | null;
  user: AuthUser | null;
}
