import { Routes } from '@angular/router';

import { authGuard } from './core/auth/guards/auth.guard';
import { guestGuard } from './core/auth/guards/guest.guard';
import { PermissionCodes } from './core/permissions/constants/permission-codes';
import { permissionGuard } from './core/permissions/guards/permission.guard';

/**
 * Uygulamanın bütün route'larıdır. Tanımlamalar menüsündeki kullanıcı
 * ekranları '/app/users' ve '/app/roles' adreslerinde açılır;
 * backend authorization API seviyesinde ayrıca uygulanır.
 */
export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/authentication/login/login.component')
        .then(component => component.LoginComponent)
  },
  {
    path: 'forgot-password',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/authentication/forgot-password/forgot-password.component')
        .then(component => component.ForgotPasswordComponent)
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./features/authentication/reset-password/reset-password.component')
        .then(component => component.ResetPasswordComponent)
  },
  {
    path: 'app',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layout/app-layout/app-layout.component')
        .then(component => component.AppLayoutComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () =>
          import('./features/home/home.component')
            .then(component => component.HomeComponent)
      },
      {
        path: 'users',
        canActivate: [permissionGuard],
        data: {
          permissionModule: 'Users'
        },
        loadComponent: () =>
          import('./features/users/user-list/user-list.component')
            .then(component => component.UserListComponent)
      },
      {
        path: 'roles',
        canActivate: [permissionGuard],
        data: {
          permissionModule: 'Roles'
        },
        loadComponent: () =>
          import('./features/roles/role-list/role-list.component')
            .then(component => component.RoleListComponent)
      },
      {
        path: 'forces-commands',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ForcesCommands' },
        loadComponent: () => import('./features/forces-commands/forces-command-list/forces-command-list.component')
          .then(component => component.ForcesCommandListComponent)
      },
      {
        path: 'definitions/forces-commands', pathMatch: 'full', redirectTo: 'forces-commands'
      },
      {
        path: 'countries',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Countries' },
        loadComponent: () =>
          import('./features/countries/country-list/country-list.component')
            .then(component => component.CountryListComponent)
      },
      {
        path: 'military-class-groups',
        canActivate: [permissionGuard],
        data: { permissionModule: 'MilitaryClassGroups' },
        loadComponent: () =>
          import('./features/military-class-groups/group-list/group-list.component')
            .then(component => component.MilitaryClassGroupListComponent)
      },
      {
        path: 'military-class-group-relations',
        canActivate: [permissionGuard],
        data: { permissionModule: 'MilitaryClassGroupRelations' },
        loadComponent: () =>
          import('./features/military-class-group-relations/relation-list/relation-list.component')
            .then(component => component.MilitaryClassGroupRelationListComponent)
      },
      {
        path: 'military-classes',
        canActivate: [permissionGuard],
        data: { permissionModule: 'MilitaryClasses' },
        loadComponent: () =>
          import('./features/military-classes/military-class-list/military-class-list.component')
            .then(component => component.MilitaryClassListComponent)
      },
      {
        path: 'branches',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Branches' },
        loadComponent: () =>
          import('./features/branches/branch-list/branch-list.component')
            .then(component => component.BranchListComponent)
      },
      {
        path: 'connected-decisions',
        canActivate: [permissionGuard],
        data: { permissionModule: 'ConnectedDecisions' },
        loadComponent: () =>
          import('./features/connected-decisions/connected-decision-list/connected-decision-list.component')
            .then(component => component.ConnectedDecisionListComponent)
      },
      {
        path: 'admission-types',
        canActivate: [permissionGuard],
        data: { permissionModule: 'AdmissionTypes' },
        loadComponent: () =>
          import('./features/admission-types/admission-type-list/admission-type-list.component')
            .then(component => component.AdmissionTypeListComponent)
      },
      {
        path: 'military-offices',
        canActivate: [permissionGuard],
        data: { permissionModule: 'MilitaryOffices' },
        loadComponent: () =>
          import('./features/military-offices/military-office-list/military-office-list.component')
            .then(component => component.MilitaryOfficeListComponent)
      },
      {
        path: 'military-schools',
        canActivate: [permissionGuard],
        data: { permissionModule: 'MilitarySchools' },
        loadComponent: () =>
          import('./features/military-schools/military-school-list/military-school-list.component')
            .then(component => component.MilitarySchoolListComponent)
      },
      {
        path: 'towns',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Towns' },
        loadComponent: () =>
          import('./features/towns/town-list/town-list.component')
            .then(component => component.TownListComponent)
      },
      {
        path: 'cities',
        canActivate: [permissionGuard],
        data: { permissionModule: 'Cities' },
        loadComponent: () =>
          import('./features/cities/city-list/city-list.component')
            .then(component => component.CityListComponent)
      },
      {
        path: 'definitions/countries', pathMatch: 'full', redirectTo: 'countries'
      },
      {
        path: 'definitions/cities', pathMatch: 'full', redirectTo: 'cities'
      },
      {
        path: 'definitions/towns', pathMatch: 'full', redirectTo: 'towns'
      },
      {
        path: 'definitions/branches', pathMatch: 'full', redirectTo: 'branches'
      },
      {
        path: 'definitions/connected-decisions', pathMatch: 'full', redirectTo: 'connected-decisions'
      },
      {
        path: 'definitions/admission-types', pathMatch: 'full', redirectTo: 'admission-types'
      },
      {
        path: 'definitions/military-offices', pathMatch: 'full', redirectTo: 'military-offices'
      },
      {
        path: 'definitions/military-schools', pathMatch: 'full', redirectTo: 'military-schools'
      },
      {
        path: 'definitions/military-classes', pathMatch: 'full', redirectTo: 'military-classes'
      },
      {
        path: 'definitions/military-class-groups', pathMatch: 'full', redirectTo: 'military-class-groups'
      },
      {
        path: 'definitions/military-class-group-relations', pathMatch: 'full',
        redirectTo: 'military-class-group-relations'
      },
      {
        path: 'purchase-numbers',
        canActivate: [permissionGuard],
        data: { permissionModule: 'PurchaseNumbers' },
        loadComponent: () =>
          import('./features/purchase-numbers/purchase-number-list/purchase-number-list.component')
            .then(component => component.PurchaseNumberListComponent)
      },
      {
        path: 'definitions/purchase-numbers', pathMatch: 'full', redirectTo: 'purchase-numbers'
      },
      {
        path: 'military-units',
        canActivate: [permissionGuard],
        data: {
          permissionModule: 'MilitaryUnits'
        },
        loadComponent: () =>
          import('./features/military-units/military-unit-list/military-unit-list.component')
            .then(component => component.MilitaryUnitListComponent)
      },
      {
        path: 'definitions/military-units',
        pathMatch: 'full',
        redirectTo: 'military-units'
      },
      {
        path: 'definitions/roles',
        pathMatch: 'full',
        redirectTo: 'roles'
      },
      {
        // Önceki /app/definitions/users favorilerini yeni adrese yönlendirir.
        path: 'definitions/users',
        pathMatch: 'full',
        redirectTo: 'users'
      },
      {
        path: 'audit-logs',
        canActivate: [permissionGuard],
        data: { permissions: [PermissionCodes.AuditLogs.View] },
        loadComponent: () =>
          import('./features/audit-logs/audit-log-list/audit-log-list.component')
            .then(component => component.AuditLogListComponent)
      },
      {
        path: 'change-password',
        loadComponent: () =>
          import('./features/authentication/change-password/change-password.component')
            .then(component => component.ChangePasswordComponent)
      }
    ]
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'app'
  },
  {
    path: '**',
    redirectTo: 'app'
  }
];
