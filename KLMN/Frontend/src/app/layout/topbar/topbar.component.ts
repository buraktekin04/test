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
  private readonly authService =
    inject(AuthService);

  private readonly router =
    inject(Router);

  @Output()
  public readonly sidebarToggle =
    new EventEmitter<void>();

  public readonly user =
    this.authService.currentUser;

  public readonly userInitials =
    computed(() => {
      const user = this.user();

      if (!user) {
        return '?';
      }

      const firstInitial =
        user.firstName?.trim()?.charAt(0) ?? '';

      const lastInitial =
        user.lastName?.trim()?.charAt(0) ?? '';

      return (firstInitial + lastInitial).toUpperCase();
    });

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

  public toggleSidebar(): void {
    this.sidebarToggle.emit();
  }

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
