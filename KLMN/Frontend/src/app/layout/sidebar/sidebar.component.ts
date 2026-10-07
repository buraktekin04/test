import {
  Component,
  EventEmitter,
  inject,
  Output
} from '@angular/core';

import {
  RouterLink,
  RouterLinkActive
} from '@angular/router';

import { NavigationService } from '../services/navigation.service';

/**
 * KLMN sidebar bileşenidir.
 */
@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  private readonly navigationService =
    inject(NavigationService);

  public readonly menuItems =
    this.navigationService.visibleItems;

  @Output()
  public readonly navigationSelected =
    new EventEmitter<void>();

  public onNavigationSelected(): void {
    this.navigationSelected.emit();
  }
}
