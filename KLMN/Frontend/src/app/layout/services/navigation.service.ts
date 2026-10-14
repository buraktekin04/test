import { computed, inject, Injectable } from '@angular/core';
import { PermissionCodes } from '../../core/permissions/constants/permission-codes';
import { PermissionService } from '../../core/permissions/services/permission.service';
import { NavigationItem } from '../models/navigation-item.model';

/**
 * Sol menüyü, gelecekte büyütülecek Tanımlamalar hiyerarşisini ve
 * her başlığın permission bazlı görünürlüğünü merkezi olarak yönetir.
 */
@Injectable({ providedIn: 'root' })
export class NavigationService {
  /** Menü yetkilerini Angular oturumundaki güncel permission'lardan hesaplar. */
  private readonly permissionService = inject(PermissionService);

  /** Ekranların menüdeki hiyerarşik gösterim sırası ve erişim şartlarıdır. */
  private readonly navigationItems: readonly NavigationItem[] = [
    {
      label: 'Ana Sayfa',
      icon: 'pi pi-home',
      route: '/app'
    },
    {
      label: 'Tanımlamalar',
      icon: 'pi pi-cog',
      children: [
        {
          label: 'Kullanıcı Tanımları',
          icon: 'pi pi-users',
          route: '/app/users',
          requiredPermissions: [PermissionCodes.Users.Query]
        },
        {
          label: 'Rol Tanımları',
          icon: 'pi pi-shield',
          route: '/app/roles',
          requiredPermissions: [PermissionCodes.Roles.Query]
        },
        {
          label: 'Kuvvet Komutanlığı Tanımları',
          icon: 'pi pi-flag',
          route: '/app/forces-commands',
          requiredPermissions: [PermissionCodes.ForcesCommands.Query]
        },
        {
          label: 'Birlik Tanımları',
          icon: 'pi pi-sitemap',
          route: '/app/military-units',
          requiredPermissions: [PermissionCodes.MilitaryUnits.View, PermissionCodes.MilitaryUnits.Query],
          permissionMode: 'any'
        }
      ]
    },
    {
      label: 'İşlem Geçmişi',
      icon: 'pi pi-history',
      route: '/app/audit-logs',
      requiredPermissions: [PermissionCodes.AuditLogs.View]
    },
    {
      label: 'İncelemeler',
      icon: 'pi pi-search',
      requiredPermissions: [PermissionCodes.Investigations.View]
    },
    {
      label: 'Raporlar',
      icon: 'pi pi-chart-bar',
      requiredPermissions: [PermissionCodes.Reports.View]
    }
  ];

  /** Kullanıcının yetkili olduğu alt menüleri içeren reaktif menü ağacıdır. */
  public readonly visibleItems = computed<readonly NavigationItem[]>(
    () => this.filterItems(this.navigationItems)
  );

  /**
   * Alt menüleri de filtreleyerek yetkisiz ve boş ana menüleri gizler.
   * Bir menünün görünmesi güvenlik sağlamaz; backend endpoint'i de yetkilidir.
   */
  private filterItems(items: readonly NavigationItem[]): NavigationItem[] {
    const result: NavigationItem[] = [];

    for (const item of items) {
      if (!this.canView(item)) continue;

      const children = item.children
        ? this.filterItems(item.children)
        : undefined;

      // Hiç görünür alt öğesi olmayan gruplanmış başlık gizlenir.
      if (item.children && !item.route && !children?.length) continue;

      result.push({ ...item, children });
    }

    return result;
  }

  /** Menü öğesinin gerekli permission kodlarının mevcut olduğunu doğrular. */
  private canView(item: NavigationItem): boolean {
    const permissions = item.requiredPermissions;

    if (!permissions?.length) return true;

    return item.permissionMode === 'any'
      ? this.permissionService.hasAny(...permissions)
      : this.permissionService.hasAll(...permissions);
  }
}
