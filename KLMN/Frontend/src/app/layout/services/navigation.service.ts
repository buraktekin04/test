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
          permissionModule: 'Users'
        },
        {
          label: 'Rol Tanımları',
          icon: 'pi pi-shield',
          route: '/app/roles',
          permissionModule: 'Roles'
        },
        {
          label: 'Kuvvet Komutanlığı Tanımları',
          icon: 'pi pi-flag',
          route: '/app/forces-commands',
          permissionModule: 'ForcesCommands'
        },
        {
          label: 'Ülke Tanımları',
          icon: 'pi pi-globe',
          route: '/app/countries',
          permissionModule: 'Countries'
        },
        {
          label: 'Şehir Tanımları',
          icon: 'pi pi-map-marker',
          route: '/app/cities',
          permissionModule: 'Cities'
        },
        {
          label: 'İlçe Tanımları',
          icon: 'pi pi-map',
          route: '/app/towns',
          permissionModule: 'Towns'
        },
        {label:'SYY Tanı Sınıflandırma Çizelgesi',icon:'pi pi-table',route:'/app/syy-classifications',permissionModule:'SYYClassifications'},
        {label:'SYY Gereken Branşlar',icon:'pi pi-sitemap',route:'/app/syy-diagnosis-branches',permissionModule:'SYYDiagnosisBranches'},
        {label:'SYY Gereken Tetkikler',icon:'pi pi-heart',route:'/app/syy-diagnosis-procedures',permissionModule:'SYYDiagnosisProcedures'},
        {label:'SYY Onay Gerektiren Maddeler',icon:'pi pi-check-square',route:'/app/syy-diagnosis-requireds',permissionModule:'SYYDiagnosisRequireds'},
        {
          label: 'SYY Tanı Listesi (M/D/F)',icon:'pi pi-sitemap',route:'/app/syy-diagnoses',permissionModule:'SYYDiagnoses'
        },
        {
          label: 'SYY Tanı Türü Tanımları',icon:'pi pi-list',route:'/app/syy-diagnosis-types',permissionModule:'SYYDiagnosisTypes'
        },
        {
          label: 'SYY Dilim Tanımları',icon:'pi pi-tags',route:'/app/syy-diagnosis-slices',permissionModule:'SYYDiagnosisSlices'
        },
        {
          label: 'Sevk Sebebi Tanımları',icon:'pi pi-file-edit',route:'/app/annotation-reasons',permissionModule:'AnnotationReasons'
        },
        {
          label: 'Uçuşa Uçak Tipi Tanımları',icon:'pi pi-send',route:'/app/plane-types',permissionModule:'PlaneTypes'
        },
        {
          label: 'Uçucu Görev Tanımları',icon:'pi pi-compass',route:'/app/flying-missions',permissionModule:'FlyingMissions'
        },
        {
          label: 'Muayene Sebebi', icon: 'pi pi-file-edit',
          route: '/app/reasons-for-examination', permissionModule: 'ReasonsForExaminations'
        },
        {
          label: 'Muayene Sebebi Rapor Türleri', icon: 'pi pi-list',
          route: '/app/reason-exam-report-types', permissionModule: 'ReasonForExamReportTypes'
        },
        {
          label: 'Kullanılmayacak Hasta Grupları', icon: 'pi pi-users',
          route: '/app/reason-exam-unused-admission-types', permissionModule: 'ReasonForExamUnusedHcAdmissionTypes'
        },
        {
          label: 'Gereken Tetkikler', icon: 'pi pi-heart',
          route: '/app/reason-exam-procedures', permissionModule: 'ReasonForExamProcedureDefs'
        },
        {
          label: 'Muayene Gereken Branşlar', icon: 'pi pi-sitemap',
          route: '/app/reason-exam-branches', permissionModule: 'ReasonForExamBranchDefs'
        },
        {
          label: 'Tetkik Türü Tanımları', icon: 'pi pi-tags',
          route: '/app/procedure-types', permissionModule: 'ProcedureTypes'
        },
        {
          label: 'Tetkik Tanımları', icon: 'pi pi-list-check',
          route: '/app/procedures', permissionModule: 'Procedures'
        },
        {
          label: 'Sağlık Tesisi Tanımları',
          icon: 'pi pi-building',
          route: '/app/hospitals',
          permissionModule: 'Hospitals'
        },
        {
          label: 'Sağlık Tesisi Rapor Türleri',
          icon: 'pi pi-building',
          route: '/app/hospital-report-types',
          permissionModule: 'HospitalReportTypes'
        },
        {
          label: 'Rapor Türü Tanımları',
          icon: 'pi pi-list',
          route: '/app/report-types',
          permissionModule: 'ReportTypes'
        },
        {
          label: 'Rütbe Grubu Tanımları',
          icon: 'pi pi-list',
          route: '/app/rank-groups',
          permissionModule: 'RankGroups'
        },
        {
          label: 'Rütbe Tanımları',
          icon: 'pi pi-list',
          route: '/app/ranks',
          permissionModule: 'Ranks'
        },
        {
          label: 'Branş Tanımları',
          icon: 'pi pi-list',
          route: '/app/branches',
          permissionModule: 'Branches'
        },
        {
          label: 'Bağlı Kurul Karar Tanımları',
          icon: 'pi pi-file',
          route: '/app/connected-decisions',
          permissionModule: 'ConnectedDecisions'
        },
        {
          label: 'Başvuru Grubu Tanımları',
          icon: 'pi pi-list-check',
          route: '/app/admission-types',
          permissionModule: 'AdmissionTypes'
        },
        {
          label: 'Askerlik Şubesi Tanımları',
          icon: 'pi pi-building',
          route: '/app/military-offices',
          permissionModule: 'MilitaryOffices'
        },
        {
          label: 'Askerî Okul Tanımları',
          icon: 'pi pi-graduation-cap',
          route: '/app/military-schools',
          permissionModule: 'MilitarySchools'
        },
        {
          label: 'Askerî Sınıf Tanımları',
          icon: 'pi pi-book',
          route: '/app/military-classes',
          permissionModule: 'MilitaryClasses'
        },
        {
          label: 'Askerî Sınıf Grubu Tanımları',
          icon: 'pi pi-clone',
          route: '/app/military-class-groups',
          permissionModule: 'MilitaryClassGroups'
        },
        {
          label: 'Sınıf–Grup Eşleştirmeleri',
          icon: 'pi pi-link',
          route: '/app/military-class-group-relations',
          permissionModule: 'MilitaryClassGroupRelations'
        },
        {
          label: 'Alım Numarası Tanımları',
          icon: 'pi pi-tags',
          route: '/app/purchase-numbers',
          permissionModule: 'PurchaseNumbers'
        },
        {
          label: 'Birlik Tanımları',
          icon: 'pi pi-sitemap',
          route: '/app/military-units',
          permissionModule: 'MilitaryUnits'
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
    if (item.permissionModule && !this.permissionService.hasModule(item.permissionModule))
      return false;

    const permissions = item.requiredPermissions;

    if (!permissions?.length) return true;

    return item.permissionMode === 'any'
      ? this.permissionService.hasAny(...permissions)
      : this.permissionService.hasAll(...permissions);
  }
}
