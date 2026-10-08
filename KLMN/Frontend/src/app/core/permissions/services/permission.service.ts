import { inject, Injectable } from '@angular/core';
import { AuthStoreService } from '../../auth/services/auth-store.service';

/**
 * UI permission kontrollerini gerçekleştirir.
 * Gerçek güvenlik backend'dedir.
 */
@Injectable({ providedIn: 'root' })
export class PermissionService {
  /** auth store durumunu veya bağımlılığını component içerisinde yönetir. */
  private readonly authStore =
    inject(AuthStoreService);

  /** UI görünürlüğü için ADMIN rolünü ve kullanıcı permission kodunu kontrol eder. */
  public has(permission: string): boolean {
    // Oturum sahibine ait profil ve role/permission verileridir.
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

  /** İstenen izinlerden en az birinin mevcut olmasını kontrol eder. */
  public hasAny(...permissions: string[]): boolean {
    return permissions.some(permission => this.has(permission));
  }

  /** İstenen izinlerin hepsinin mevcut olmasını kontrol eder. */
  public hasAll(...permissions: string[]): boolean {
    return permissions.every(permission => this.has(permission));
  }
}

