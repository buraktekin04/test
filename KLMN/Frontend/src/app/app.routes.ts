import { Routes } from '@angular/router';

import { authGuard } from './core/auth/guards/auth.guard';
import { guestGuard } from './core/auth/guards/guest.guard';
import { PermissionCodes } from './core/permissions/constants/permission-codes';
import { permissionGuard } from './core/permissions/guards/permission.guard';

/**
 * Uygulamanın bütün route'larıdır. Tanımlamalar alt ekranları
 * '/app/definitions' altında gruplandırılır; backend authorization
 * ayrıca API endpoint seviyesinde uygulanmaya devam eder.
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
        path: 'definitions/users',
        canActivate: [permissionGuard],
        data: {
          permissions: [PermissionCodes.Users.Query]
        },
        loadComponent: () =>
          import('./features/users/user-list/user-list.component')
            .then(component => component.UserListComponent)
      },
      {
        // Eski URL ile kaydedilmiş tarayıcı favorilerini bozmuyoruz.
        path: 'users',
        pathMatch: 'full',
        redirectTo: 'definitions/users'
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
