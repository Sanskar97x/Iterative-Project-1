import { Component, inject, effect } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth';
import { NotificationService } from './core/services/notification.service';
import { HeaderComponent } from './shared/header.component';

@Component({
  imports: [RouterOutlet, HeaderComponent],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);

  constructor() {
    effect(() => {
      if (this.authService.isLoggedIn()) {
        this.notificationService.start();
      } else {
        this.notificationService.stop();
      }
    });
  }
}