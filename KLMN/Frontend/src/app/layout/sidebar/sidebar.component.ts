import { Component, EventEmitter, inject, Output, signal } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NavigationService } from '../services/navigation.service';

/**
 * İç içe tanım ekranlarını açılıp kapanan menü gruplarında gösterir.
 * Mobil ekranda route seçilince layout'a kapatma olayı gönderir.
 */
@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, NgTemplateOutlet],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  /** Yetkilendirilmiş menü ağacını sağlayan merkezi servistir. */
  private readonly navigationService = inject(NavigationService);

  /** Ekrana basılan kullanıcının yetkili olduğu menü öğeleridir. */
  public readonly menuItems = this.navigationService.visibleItems;

  /** Varsayılan açık ana menülerin isimlerini içeren reaktif kümedir. */
  private readonly expandedMenus = signal<ReadonlySet<string>>(
    new Set(['Tanımlamalar'])
  );

  /** Mobil sidebar'a menü seçildiğinin bilgisini iletir. */
  @Output()
  public readonly navigationSelected = new EventEmitter<void>();

  /** Kullanıcının ana menüyü açıp kapatmasını sağlar. */
  public toggleGroup(label: string): void {
    this.expandedMenus.update(current => {
      const updated = new Set(current);
      if (updated.has(label)) updated.delete(label);
      else updated.add(label);
      return updated;
    });
  }

  /** İç içe menünün açık olup olmadığını belirtir. */
  public isExpanded(label: string): boolean {
    return this.expandedMenus().has(label);
  }

  /** Bağlantı seçildiğinde mobil menüyü kapatır. */
  public onNavigationSelected(): void {
    this.navigationSelected.emit();
  }
}
