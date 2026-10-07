import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/**
 * KLMN root component'idir.
 */
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `<router-outlet />`
})
export class AppComponent {
}
