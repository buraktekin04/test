import {
  Component,
  computed,
  EventEmitter,
  inject,
  Output
} from '@angular/core';

import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';

import { AuthService } from '../../core/auth/services/auth.service';

/**
 * KLMN topbar bileşenidir.
 */
@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [
    AvatarModule,
    ButtonModule,
    MenuModule
  ],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss'
})
export class TopbarComponent {
  /** Oturum durumu, kullanıcı profili ve çıkış işlemlerini yöneten servistir. */
  private readonly authService =
    inject(AuthService);

  /** Kullanıcıyı giriş, ana sayfa veya yetkili ekranlara yönlendirir. */
  private readonly router =
    inject(Router);

  @Output()
  public readonly sidebarToggle =
    new EventEmitter<void>();

  /** Güncel oturum ve rol kontrollerinde kullanılan kullanıcı bilgileridir. */
  public readonly user =
    this.authService.currentUser;

  /** user initials alanını component veya servis durumunda kullanır. */
  public readonly userInitials =
    computed(() => {
      // Güncel oturum ve rol kontrollerinde kullanılan kullanıcı bilgileridir.
      const user = this.user();

      if (!user) {
        return '?';
      }

      // first initial değerini ekranın sonraki filtreleme veya yönlendirme adımlarında kullanır.
      const firstInitial =
        user.firstName?.trim()?.charAt(0) ?? '';

      // last initial değerini ekranın sonraki filtreleme veya yönlendirme adımlarında kullanır.
      const lastInitial =
        user.lastName?.trim()?.charAt(0) ?? '';

      return (firstInitial + lastInitial).toUpperCase();
    });

  /** user menu items alanını component veya servis durumunda kullanır. */
  public readonly userMenuItems: MenuItem[] = [
    {
      label: 'Parola Değiştir',
      icon: 'pi pi-key',
      command: () => {
        void this.router.navigate(['/app/change-password']);
      }
    },
    {
      separator: true
    },
    {
      label: 'Çıkış Yap',
      icon: 'pi pi-sign-out',
      command: () => this.logout()
    }
  ];

  /** toggle sidebar işlemini kullanıcı etkileşimi ve servis sonucuna göre yürütür. */
  public toggleSidebar(): void {
    this.sidebarToggle.emit();
  }

  /** Mevcut oturum cookie'sini sunucuda iptal ederek kullanıcıyı giriş ekranına yönlendirir. */
  private logout(): void {
    this.authService.logout()
      .subscribe({
        next: () => {
          void this.router.navigate(
            ['/login'],
            {
              queryParams: {
                loggedOut: true
              }
            }
          );
        },
        error: error => {
          console.error('[KLMN AUTH] Logout error:', error);
          void this.router.navigate(['/login']);
        }
      });
  }
}
