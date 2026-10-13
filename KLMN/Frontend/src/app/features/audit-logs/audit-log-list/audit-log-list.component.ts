import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { AuditLogService } from '../../../core/auditing/audit-log.service';
import { AuditLogItem, AuditLogQuery } from '../../../core/auditing/audit-log.model';

/** Tanımlamaların tamamını kapsayan, salt okunur genel audit geçmiş ekranıdır. */
@Component({
  selector: 'app-audit-log-list',
  standalone: true,
  imports: [DatePipe, ReactiveFormsModule, ButtonModule, TableModule, DialogModule, TagModule],
  templateUrl: './audit-log-list.component.html',
  styleUrl: './audit-log-list.component.scss'
})
export class AuditLogListComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  public readonly auditService = inject(AuditLogService);

  public readonly form = this.fb.nonNullable.group({
    entityType: '',
    entityId: '',
    action: '',
    from: '',
    to: ''
  });
  public readonly rows = signal<AuditLogItem[]>([]);
  public readonly total = signal(0);
  public readonly page = signal(1);
  public readonly pageSize = 20;
  public readonly totalPages = computed(() => Math.max(1, Math.ceil(this.total() / this.pageSize)));
  public readonly loading = signal(false);
  public readonly error = signal<string | null>(null);
  public readonly selected = signal<AuditLogItem | null>(null);
  private requestVersion = 0;

  /** Satırdaki Geçmiş kısayoluyla aynı sayfa isteğe bağlı ön filtre ile açılabilir. */
  public ngOnInit(): void {
    const params = this.route.snapshot.queryParamMap;
    this.form.patchValue({
      entityType: params.get('entityType') ?? '',
      entityId: params.get('entityId') ?? ''
    });
    this.load();
  }

  /** Sadece geçerli filtreleri kullanarak sunucu sayfalaması uygular. */
  public load(): void {
    const raw = this.form.getRawValue();
    const id = raw.entityId.trim();
    if (id && (!/^[0-9a-fA-F-]{36}$/.test(id) || !raw.entityType)) {
      this.error.set('Kayıt kimliğiyle aramada geçerli tür ve GUID giriniz.');
      return;
    }
    const from = raw.from ? new Date(raw.from).toISOString() : undefined;
    const to = raw.to ? new Date(raw.to).toISOString() : undefined;

    const query: AuditLogQuery = {
      entityType: raw.entityType || undefined,
      entityId: id || undefined,
      action: raw.action || undefined,
      from, to,
      pageNumber: this.page(), pageSize: this.pageSize
    };
    const version = ++this.requestVersion;
    this.loading.set(true);
    this.error.set(null);
    this.auditService.list(query).pipe(finalize(() => {
      if (version === this.requestVersion) this.loading.set(false);
    })).subscribe({
      next: data => {
        if (version !== this.requestVersion) return;
        this.rows.set(data.items);
        this.total.set(data.totalCount);
      },
      error: (err: HttpErrorResponse) => {
        if (version !== this.requestVersion) return;
        this.error.set(err.error?.detail || 'İşlem geçmişi alınamadı.');
      }
    });
  }

  /** Arama sonrasında ilk sayfaya döner. */
  public search(): void { this.page.set(1); this.load(); }

  /** Filtreleri ve listeyi temizler. */
  public clear(): void {
    this.form.reset({ entityType: '', entityId: '', action: '', from: '', to: '' });
    this.search();
  }

  /** Önceki/sonraki sayfayı yükler. */
  public go(delta: number): void {
    const next = this.page() + delta;
    if (next < 1 || next > this.totalPages()) return;
    this.page.set(next);
    this.load();
  }

  /** Tek satırda eski/yeni değerlerin ayrıntılarını açar. */
  public openDetails(item: AuditLogItem): void { this.selected.set(item); }
  public closeDetails(): void { this.selected.set(null); }
  public changes(item: AuditLogItem) { return this.auditService.changes(item); }

  public label(action: string): string {
    const names: Record<string, string> = {
      Create: 'Oluşturma', Update: 'Güncelleme',
      Delete: 'Silme', Activate: 'Aktifleştirme', Deactivate: 'Pasifleştirme'
    };
    return names[action] ?? action;
  }
}
