import { Component, computed, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { APP_BRANDING } from '../../core/config/app-branding';
import { NavigationItem } from '../models/navigation-item.model';
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
  /** Kullanıcı yalnızca yetkili olduğu menülerde arama yapabilir. */
  public readonly searchTerm = signal('');

  /** Türkçe karakterlere uygun, alt gruplarda da çalışan arama sonucu. */
  public readonly menuItems = computed(() => {
    const allowed = this.navigationService.visibleItems();
    const term = this.searchTerm().trim().toLocaleLowerCase('tr-TR');
    return term ? this.filterMenu(allowed, term) : allowed;
  });

  private filterMenu(items: readonly NavigationItem[], term: string): NavigationItem[] {
    return items.flatMap(item => {
      if (item.label.toLocaleLowerCase('tr-TR').includes(term)) return [item];
      const children = item.children ? this.filterMenu(item.children, term) : [];
      return children.length ? [{ ...item, children }] : [];
    });
  }

  private readonly expandedMenus = signal<ReadonlySet<string>>(new Set(['root/Tanımlamalar']));

  /** Her grup kendi tam hiyerarşik yoluyla tutulur; aynı adlı alt/üst menüler çakışmaz. */
  public groupKey(parentKey: string, label: string): string {
    return parentKey + '/' + label;
  }

  public toggleGroup(label: string): void {
    if (this.collapsed) this.expandRequested.emit();
    this.expandedMenus.update(current => {
      const next = new Set(current);
      if (next.has(label)) next.delete(label);
      else next.add(label);
      return next;
    });
  }

  public isExpanded(label: string): boolean { return this.searchTerm().trim().length > 0 || this.expandedMenus().has(label); }
  public onNavigationSelected(): void {
    this.navigationSelected.emit();
  }
}
