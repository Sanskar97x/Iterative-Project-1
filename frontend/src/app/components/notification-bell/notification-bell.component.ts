import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';
import { AppNotification } from '../../core/models/notification.model';

@Component({
  selector: 'app-notification-bell',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-bell.component.html',
  styleUrls: ['./notification-bell.component.css']
})
export class NotificationBellComponent {
  notificationService = inject(NotificationService);
  isOpen = false;

  toggleDropdown(): void {
    this.isOpen = !this.isOpen;
  }

onToggleRead(n: AppNotification): void {
  console.log('clicked', n);
  if (n.isRead) {
    this.notificationService.markAsUnread(n.id);
  } else {
    this.notificationService.markAsRead(n.id);
  }
}

  onNotificationClick(id: number, isRead: boolean): void {
    if (!isRead) {
      this.notificationService.markAsRead(id);
    }
  }

  onMarkAllRead(): void {
    this.notificationService.markAllAsRead();
  }

 onMarkAsUnread(id: number): void {
  console.log('Clicked',id);
  this.notificationService.markAsUnread(id);
}
}