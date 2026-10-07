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
  private readonly userService = inject(UserService);
  private readonly formBuilder = inject(FormBuilder);

  public readonly users = signal<UserListItem[]>([]);
  public readonly totalCount = signal(0);
  public readonly pageNumber = signal(1);
  public readonly pageSize = signal(20);
  public readonly loading = signal(false);
  public readonly errorMessage = signal<string | null>(null);

  public readonly filterForm =
    this.formBuilder.nonNullable.group({
      search: [''],
      includeInactive: [false]
    });

  public ngOnInit(): void {
    this.loadUsers();
  }

  public loadUsers(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

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
          console.error('[MSKBS USERS] User query error:', error);

          this.users.set([]);
          this.totalCount.set(0);

          this.errorMessage.set(
            error.error?.detail ??
            'Kullanıcılar yüklenirken bir hata oluştu.'
          );
        }
      });
  }

  public search(): void {
    this.pageNumber.set(1);
    this.loadUsers();
  }

  public clearFilters(): void {
    this.filterForm.reset({
      search: '',
      includeInactive: false
    });

    this.pageNumber.set(1);
    this.loadUsers();
  }

  public onPageChange(event: PaginatorState): void {
    const rows = event.rows ?? this.pageSize();
    const first = event.first ?? 0;

    this.pageSize.set(rows);
    this.pageNumber.set(
      Math.floor(first / rows) + 1
    );

    this.loadUsers();
  }
}
