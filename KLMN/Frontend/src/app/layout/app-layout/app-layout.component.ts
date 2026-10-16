import { Component, HostListener, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';

/** Mobil çekmece ve masaüstü dar menü durumunu birbirinden bağımsız yönetir. */
@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, TopbarComponent],
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.scss'
})
export class AppLayoutComponent {
  public readonly sidebarOpen = signal(false);
  public readonly sidebarCollapsed = signal(false);

  public toggleSidebar(): void {
    if (typeof window !== 'undefined' && window.matchMedia('(max-width: 992px)').matches) {
      this.sidebarOpen.update(value => !value);
    } else {
      this.sidebarCollapsed.update(value => !value);
    }
  }

  public expandSidebar(): void {
    this.sidebarCollapsed.set(false);
    this.sidebarOpen.set(
      typeof window !== 'undefined' &&
      window.matchMedia('(max-width: 992px)').matches
    );
  }

  public closeSidebar(): void {
    this.sidebarOpen.set(false);
  }

  @HostListener('document:keydown.escape')
  public onEscape(): void {
    this.closeSidebar();
  }
}
