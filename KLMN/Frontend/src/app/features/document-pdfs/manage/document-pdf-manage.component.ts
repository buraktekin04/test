import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';
import { ConfirmationService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TableModule } from 'primeng/table';
import { DocumentPdf } from '../models/document-pdf.model';
import { DocumentPdfService } from '../services/document-pdf.service';

@Component({
  selector: 'app-document-pdf-manage',
  standalone: true,
  imports: [DatePipe, DecimalPipe, ButtonModule, ConfirmDialogModule, TableModule],
  providers: [ConfirmationService],
  templateUrl: './document-pdf-manage.component.html',
  styleUrl: './document-pdf-manage.component.scss'
})
export class DocumentPdfManageComponent implements OnInit {
  @ViewChild('pdfInput') private pdfInput?: ElementRef<HTMLInputElement>;

  private readonly service = inject(DocumentPdfService);
  private readonly confirmations = inject(ConfirmationService);
  public readonly documents = signal<DocumentPdf[]>([]);
  public readonly selectedFile = signal<File | null>(null);
  public readonly loading = signal(false);
  public readonly saving = signal(false);
  public readonly deletingId = signal<string | null>(null);
  public readonly errorMessage = signal<string | null>(null);
  public readonly successMessage = signal<string | null>(null);

  public ngOnInit(): void { this.load(); }

  public load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);
    this.service.list().pipe(finalize(() => this.loading.set(false))).subscribe({
      next: files => this.documents.set(files),
      error: () => this.errorMessage.set('Dokümanlar yüklenemedi. Tekrar deneyin.')
    });
  }

  public selectFile(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.item(0) ?? null;
    this.selectedFile.set(null);
    this.errorMessage.set(null);
    if (!file) return;
    if (!file.name.toLowerCase().endsWith('.pdf') || file.size === 0) {
      this.errorMessage.set('Geçerli bir PDF dosyası seçin.');
      return;
    }
    this.selectedFile.set(file);
  }

  public upload(): void {
    const file = this.selectedFile();
    if (!file || this.saving()) return;
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.saving.set(true);
    this.service.upload(file).pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.selectedFile.set(null);
        if (this.pdfInput) this.pdfInput.nativeElement.value = '';
        this.successMessage.set('Doküman yüklendi.');
        this.load();
      },
      error: err => this.errorMessage.set(this.errorText(err, 'Doküman yüklenemedi.'))
    });
  }

  public view(item: DocumentPdf): void {
    this.errorMessage.set(null);
    this.service.openPdf(item.id, () => this.errorMessage.set(
      'PDF açılamadı. Bağlantıyı ve açılır pencere iznini kontrol edin.'));
  }

  public confirmDelete(item: DocumentPdf): void {
    if (this.deletingId()) return;
    this.confirmations.confirm({
      header: 'Dokümanı Sil',
      message: '“' + item.fileName + '” silinsin mi?',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sil', rejectLabel: 'Vazgeç',
      accept: () => this.delete(item)
    });
  }

  private delete(item: DocumentPdf): void {
    this.deletingId.set(item.id);
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.service.delete(item.id, item.version)
      .pipe(finalize(() => this.deletingId.set(null))).subscribe({
        next: () => { this.successMessage.set('Doküman silindi.'); this.load(); },
        error: err => this.errorMessage.set(this.errorText(err, 'Doküman silinemedi.'))
      });
  }

  private errorText(error: unknown, fallback: string): string {
    return error instanceof HttpErrorResponse && typeof error.error?.detail === 'string'
      ? error.error.detail : fallback;
  }
}

