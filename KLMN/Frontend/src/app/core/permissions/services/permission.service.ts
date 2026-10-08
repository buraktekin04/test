import { inject, Injectable } from '@angular/core';
import { AuthStoreService } from '../../auth/services/auth-store.service';

/**
 * UI permission kontrollerini gerçekleştirir.
 * Gerçek güvenlik backend'dedir.
 */
@Injectable({ providedIn: 'root' })
export class PermissionService {
  private readonly authStore =
    inject(AuthStoreService);

  public has(permission: string): boolean {
    const user = this.authStore.user();

    if (!user) {
      return false;
    }

    if (user.roles.some(role => role.toUpperCase() === 'ADMIN')) {
      return true;
    }

    return user.permissions.some(
      granted => granted.toLowerCase() === permission.toLowerCase()
    );
  }

  public hasAny(...permissions: string[]): boolean {
    return permissions.some(permission => this.has(permission));
  }

  public hasAll(...permissions: string[]): boolean {
    return permissions.every(permission => this.has(permission));
  }
}

