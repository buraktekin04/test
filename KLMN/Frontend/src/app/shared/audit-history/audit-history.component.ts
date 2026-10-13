import { Component, Input, OnChanges, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { finalize } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { AuditLogItem } from '../../core/auditing/audit-log.model';
import { AuditLogService } from '../../core/auditing/audit-log.service';

/** Her tanım ekranında kullanılabilecek, tek kayda ait geçmiş listesidir. */
@Component({
  selector: 'app-audit-history',
  standalone: true,
  imports: [DatePipe, ButtonModule, TagModule],
  templateUrl: './audit-history.component.html',
  styleUrl: './audit-history.component.scss'
})
export class AuditHistoryComponent implements OnChanges {
  private readonly auditService = inject(AuditLogService);

  @Input({ required: true }) public entityType = '';
  @Input({ required: true }) public entityId = '';

  public readonly entries = signal<AuditLogItem[]>([]);
  public readonly total = signal(0);
  public readonly totalPages = computed(() => Math.max(1, Math.ceil(this.total() / 10)));
  public readonly page = signal(1);
  public readonly loading = signal(false);
  public readonly error = signal<string | null>(null);
  private requestVersion = 0;

  /** Satır veya varlık değiştiğinde önceki geçmiş bilgilerini temizleyerek yeni kayıtları getirir. */
  public ngOnChanges(): void {
    this.page.set(1);
    this.load();
  }

  /** Sayfalanmış satır geçmişi, kullanıcıdan bağımsız API yetkisiyle okunur. */
  public load(): void {
    if (!this.entityType || !this.entityId) return;
    const requestVersion = ++this.requestVersion;
    this.loading.set(true);
    this.error.set(null);
    this.auditService.list({
      entityType: this.entityType, entityId: this.entityId,
      pageNumber: this.page(), pageSize: 10
    }).pipe(finalize(() => {
      if (requestVersion === this.requestVersion) this.loading.set(false);
    })).subscribe({
      next: data => {
        if (requestVersion !== this.requestVersion) return;
        this.entries.set(data.items);
        this.total.set(data.totalCount);
      },
      error: () => {
        if (requestVersion !== this.requestVersion) return;
        this.entries.set([]);
        this.error.set('İşlem geçmişi yüklenemedi. Lütfen tekrar deneyiniz.');
      }
    });
  }

  /** Sadece bir sonraki/önceki kayıt sayfasına geçiş yapar. */
  public go(delta: number): void {
    const next = this.page() + delta;
    if (next < 1 || next > Math.max(1, Math.ceil(this.total() / 10))) return;
    this.page.set(next);
    this.load();
  }

  /** Ekranda yalnızca önceden seçilmiş güvenli audit alanları gösterilir. */
  public changes(entry: AuditLogItem) { return this.auditService.changes(entry); }

  /** İlişki kaldırma hareketlerini genel bir Pasifleştirme etiketi yerine açıklar. */
  public actionLabel(entry: AuditLogItem): string {
    return this.auditService.actionLabel(entry);
  }
}
