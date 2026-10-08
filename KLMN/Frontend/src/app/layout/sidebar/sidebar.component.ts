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
  /** navigation service alanını component veya servis durumunda kullanır. */
  private readonly navigationService =
    inject(NavigationService);

  /** menu items alanını component veya servis durumunda kullanır. */
  public readonly menuItems =
    this.navigationService.visibleItems;

  @Output()
  public readonly navigationSelected =
    new EventEmitter<void>();

  /** on navigation selected işlemini kullanıcı etkileşimi ve servis sonucuna göre yürütür. */
  public onNavigationSelected(): void {
    this.navigationSelected.emit();
  }
}
