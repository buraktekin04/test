import {
  computed,
  inject,
  Injectable
} from '@angular/core';

import { PermissionCodes } from '../../core/permissions/constants/permission-codes';
import { PermissionService } from '../../core/permissions/services/permission.service';
import { NavigationItem } from '../models/navigation-item.model';

/**
 * Sidebar menüsünü merkezi olarak tanımlar ve permission'a göre filtreler.
 */
@Injectable({ providedIn: 'root' })
export class NavigationService {
  /** Menü, guard ve butonların yetki görünürlüğünü hesaplayan servistir. */
  private readonly permissionService =
    inject(PermissionService);

  /** Sol menüde yer alabilecek sayfaların yetki ve route metadata listesidir. */
  private readonly navigationItems:
    readonly NavigationItem[] = [
    {
      label: 'Ana Sayfa',
      icon: 'pi pi-home',
      route: '/app'
    },
    {
      label: 'Kullanıcılar',
      icon: 'pi pi-users',
      route: '/app/users',
      requiredPermissions: [
        PermissionCodes.Users.Query
      ]
    },
    {
      label: 'Roller',
      icon: 'pi pi-shield',
      requiredPermissions: [
        PermissionCodes.Roles.View
      ]
    },
    {
      label: 'Organizasyon',
      icon: 'pi pi-sitemap',
      requiredPermissions: [
        PermissionCodes.Organizations.View
      ]
    },
    {
      label: 'İncelemeler',
      icon: 'pi pi-search',
      requiredPermissions: [
        PermissionCodes.Investigations.View
      ]
    },
    {
      label: 'Raporlar',
      icon: 'pi pi-chart-bar',
      requiredPermissions: [
        PermissionCodes.Reports.View
      ]
    }
  ];

  /** Geçerli role ve permission'lara göre görülebilir menü öğelerini hesaplar. */
  public readonly visibleItems =
    computed<readonly NavigationItem[]>(
      () => this.filterItems(this.navigationItems)
    );

  /** Hiyerarşik menü öğelerini kullanıcı izinlerine göre rekürsif olarak filtreler. */
  private filterItems(
    items: readonly NavigationItem[]
  ): NavigationItem[] {
    // Permission filtrelemesinden geçen görünür menü öğelerinin listesidir.
    const result: NavigationItem[] = [];

    for (const item of items) {
      if (!this.canView(item)) {
        continue;
      }

      // Alt menüde gruplanmış iç içe navigasyon öğeleridir.
      const children =
        item.children
          ? this.filterItems(item.children)
          : undefined;

      if (
        item.children &&
        !item.route &&
        (!children || children.length === 0)
      ) {
        continue;
      }

      result.push({
        ...item,
        children
      });
    }

    return result;
  }

  /** Menü öğesinin gerekli yetkilerinin mevcut kullanıcıda bulunup bulunmadığını belirler. */
  private canView(item: NavigationItem): boolean {
    // Bir menü veya route'un gerektirdiği izin kodlarıdır.
    const permissions = item.requiredPermissions;

    if (!permissions || permissions.length === 0) {
      return true;
    }

    return item.permissionMode === 'any'
      ? this.permissionService.hasAny(...permissions)
      : this.permissionService.hasAll(...permissions);
  }
}
