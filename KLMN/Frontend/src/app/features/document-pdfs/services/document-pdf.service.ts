import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../../core/config/api.tokens';
import { DocumentPdf } from '../models/document-pdf.model';

@Injectable({ providedIn: 'root' })
export class DocumentPdfService {
  private readonly http = inject(HttpClient);
  private readonly url = inject(API_BASE_URL) + '/document-pdfs';

  public list(): Observable<DocumentPdf[]> {
    return this.http.get<DocumentPdf[]>(this.url);
  }
  public upload(file: File): Observable<DocumentPdf> {
    const data = new FormData();
    data.append('file', file, file.name);
    return this.http.post<DocumentPdf>(this.url, data);
  }
  public getFile(id: string): Observable<Blob> {
    return this.http.get(this.url + '/' + encodeURIComponent(id) + '/file',
      { responseType: 'blob' });
  }
  public delete(id: string, version: number): Observable<void> {
    return this.http.delete<void>(this.url + '/' + encodeURIComponent(id),
      { params: new HttpParams().set('version', version) });
  }
  /** Tarayıcı PDF'yi JWT ile indirir; yeni sekmeye geçici blob URL verilir. */
  public openPdf(id: string, onError: () => void): void {
    const popup = window.open('about:blank', '_blank');
    if (!popup) { onError(); return; }
    popup.opener = null;
    this.getFile(id).subscribe({
      next: data => {
        if (popup.closed || !data.size) { popup.close(); onError(); return; }
        const link = URL.createObjectURL(new Blob([data], { type: 'application/pdf' }));
        popup.location.href = link;
        setTimeout(() => URL.revokeObjectURL(link), 10 * 60 * 1000);
      },
      error: () => { popup.close(); onError(); }
    });
  }
}

