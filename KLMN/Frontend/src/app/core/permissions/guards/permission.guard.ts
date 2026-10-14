import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PermissionService } from '../services/permission.service';

/**
 * Route seviyesinde permission kontrolü yapar.
 */
export const permissionGuard: CanActivateFn =
  route => {
    // Ekranların yetki görünürlüğü için kullanılan permission kontrol servisidir.
    const permissionService = inject(PermissionService);
    // Başarılı işlem veya oturum kaybı sonrası ekran geçişlerini gerçekleştirir.
    const router = inject(Router);

    // Tanım ekranına girişte aynı modülün herhangi bir yetkisi yeterlidir.
    const permissionModule = route.data['permissionModule'] as string | undefined;
    if (permissionModule) {
      return permissionService.hasModule(permissionModule)
        ? true
        : router.createUrlTree(['/app']);
    }

    // Kullanıcının etkin permission kodlarını içerir.
    const permissions =
      route.data['permissions'] as string[] | undefined;

    // Çoklu permission kontrolünün any veya all şeklinde yapılacağını belirler.
    const mode =
      route.data['permissionMode'] as 'all' | 'any' | undefined;

    if (!permissions || permissions.length === 0) {
      return true;
    }

    // Kullanıcının route erişim kurallarını sağlayıp sağlamadığını belirtir.
    const authorized =
      mode === 'any'
        ? permissionService.hasAny(...permissions)
        : permissionService.hasAll(...permissions);

    return authorized
      ? true
      : router.createUrlTree(['/app']);
  };
