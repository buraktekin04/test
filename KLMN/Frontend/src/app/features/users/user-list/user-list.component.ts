import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';
import { DatePipe } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule
} from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import {
  PaginatorModule,
  PaginatorState
} from 'primeng/paginator';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { UserListItem } from '../models/user-list-item.model';
import { UserService } from '../services/user.service';

/**
 * Kullanıcı listeleme ve sorgulama ekranıdır.
 */
@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    DatePipe,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    CheckboxModule,
    PaginatorModule,
    MessageModule
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit {
  /** Kullanıcı listeleme sorgularını yöneten REST API servisidir. */
  private readonly userService = inject(UserService);
  /** Arama ve filtreleme için reactive form alanlarını oluşturan servistir. */
  private readonly formBuilder = inject(FormBuilder);

  /** Kullanıcı listeleme ekranında gösterilecek kayıtları tutan signal'dır. */
  public readonly users = signal<UserListItem[]>([]);
  /** Filtrelenmiş kullanıcı kayıtlarının toplam sayısını tutan signal'dır. */
  public readonly totalCount = signal(0);
  /** Bir tabanlı mevcut sayfa numarasıdır. */
  public readonly pageNumber = signal(1);
  /** Sayfa başına alınacak kullanıcı kayıt sayısıdır. */
  public readonly pageSize = signal(20);
  /** Liste veya form yüklenirken arayüzün durumunu bildiren signal'dır. */
  public readonly loading = signal(false);
  /** API hata durumunda kullanıcıya gösterilen hata metnini tutan signal'dır. */
  public readonly errorMessage = signal<string | null>(null);

  /** Kullanıcı arama metni ve pasifleri dahil etme seçimlerini tutan formdur. */
  public readonly filterForm =
    this.formBuilder.nonNullable.group({
      search: [''],
      includeInactive: [false]
    });

  /** Component ilk açıldığında gerekli kullanıcı listesini veya ekran verisini yükler. */
  public ngOnInit(): void {
    this.loadUsers();
  }

  /** Seçili filtrelerle API sorgusu başlatır; sonuç veya hata signal'larını günceller. */
  public loadUsers(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    // Reactive formdan okunan güncel kullanıcı listeleme filtreleridir.
    const filters = this.filterForm.getRawValue();

    this.userService
      .getUsers({
        search: filters.search,
        includeInactive: filters.includeInactive,
        pageNumber: this.pageNumber(),
        pageSize: this.pageSize()
      })
      .pipe(
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: response => {
          this.users.set(response.items);
          this.totalCount.set(response.totalCount);
        },

        error: (error: HttpErrorResponse) => {
          console.error('[KLMN USERS] User query error:', error);

          this.users.set([]);
          this.totalCount.set(0);

          this.errorMessage.set(
            error.error?.detail ??
            'Kullanıcılar yüklenirken bir hata oluştu.'
          );
        }
      });
  }

  /** Kullanıcı listesi için uygulanacak serbest metin filtre kriteridir. */
  public search(): void {
    this.pageNumber.set(1);
    this.loadUsers();
  }

  /** Arama kriterlerini varsayılan duruma sıfırlar ve listeyi yeniler. */
  public clearFilters(): void {
    this.filterForm.reset({
      search: '',
      includeInactive: false
    });

    this.pageNumber.set(1);
    this.loadUsers();
  }

  /** Paginator değişikliğinden sayfa boyutu ve numarasını hesaplayarak sorgular. */
  public onPageChange(event: PaginatorState): void {
    // Paginator olayından alınan yeni sayfa başına satır sayısıdır.
    const rows = event.rows ?? this.pageSize();
    // Paginator olayında gösterilen ilk kaydın sıfır tabanlı indeksidir.
    const first = event.first ?? 0;

    this.pageSize.set(rows);
    this.pageNumber.set(
      Math.floor(first / rows) + 1
    );

    this.loadUsers();
  }
}
