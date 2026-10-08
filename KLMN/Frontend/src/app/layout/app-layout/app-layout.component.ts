import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';

/**
 * Authentication sonrası ana uygulama layout'udur.
 */
@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    SidebarComponent,
    TopbarComponent
  ],
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.scss'
})
export class AppLayoutComponent {
  /** sidebar open alanını component veya servis durumunda kullanır. */
  public readonly sidebarOpen = signal(false);

  /** toggle sidebar işlemini kullanıcı etkileşimi ve servis sonucuna göre yürütür. */
  public toggleSidebar(): void {
    this.sidebarOpen.update(current => !current);
  }

  /** close sidebar işlemini kullanıcı etkileşimi ve servis sonucuna göre yürütür. */
  public closeSidebar(): void {
    this.sidebarOpen.set(false);
  }
}
