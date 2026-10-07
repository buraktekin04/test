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
  public readonly sidebarOpen = signal(false);

  public toggleSidebar(): void {
    this.sidebarOpen.update(current => !current);
  }

  public closeSidebar(): void {
    this.sidebarOpen.set(false);
  }
}
