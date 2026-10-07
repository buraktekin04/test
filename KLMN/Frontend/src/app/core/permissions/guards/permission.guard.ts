import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionService } from '../services/permission.service';

/**
 * Route seviyesinde permission kontrolü yapar.
 */
export const permissionGuard: CanActivateFn =
  route => {
    const permissionService = inject(PermissionService);
    const router = inject(Router);

    const permissions =
      route.data['permissions'] as string[] | undefined;

    const mode =
      route.data['permissionMode']
        as 'all' | 'any' | undefined;

    if (!permissions || permissions.length === 0) {
      return true;
    }

    const authorized =
      mode === 'any'
        ? permissionService.hasAny(...permissions)
        : permissionService.hasAll(...permissions);

    return authorized
      ? true
      : router.createUrlTree(['/app']);
  };
