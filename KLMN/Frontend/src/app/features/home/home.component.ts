import { Component, inject } from '@angular/core';
import { CardModule } from 'primeng/card';
import { AuthService } from '../../core/auth/services/auth.service';

/**
 * Ana uygulama giriş ekranıdır.
 */
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CardModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  /** Oturum durumu, kullanıcı profili ve çıkış işlemlerini yöneten servistir. */
  private readonly authService = inject(AuthService);

  /** Güncel oturum ve rol kontrollerinde kullanılan kullanıcı bilgileridir. */
  public readonly user =
    this.authService.currentUser;
}
