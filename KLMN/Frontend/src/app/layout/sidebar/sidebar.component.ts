import { Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { APP_BRANDING } from '../../core/config/app-branding';
import { NavigationService } from '../services/navigation.service';

/** Menünün içeriği permission bazlıdır, genişliği üst layout tarafından yönetilir. */
@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, NgTemplateOutlet],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  private readonly navigationService = inject(NavigationService);

  @Input() public collapsed = false;
  @Output() public readonly navigationSelected = new EventEmitter<void>();
  @Output() public readonly expandRequested = new EventEmitter<void>();

  public readonly branding = APP_BRANDING;
  public readonly menuItems = this.navigationService.visibleItems;
  private readonly expandedMenus = signal<ReadonlySet<string>>(new Set(['Tanımlamalar']));

  public toggleGroup(label: string): void {
    if (this.collapsed) {
      this.expandRequested.emit();
      if (this.isExpanded(label)) return;
    }
    this.expandedMenus.update(current => {
      const next = new Set(current);
      if (next.has(label)) next.delete(label);
      else next.add(label);
      return next;
    });
  }

  public isExpanded(label: string): boolean { return this.expandedMenus().has(label); }
  public onNavigationSelected(): void { this.navigationSelected.emit(); }
}
