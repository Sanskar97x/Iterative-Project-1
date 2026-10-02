import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { AuthService } from './auth';
import { AppNotification } from '../models/notification.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private hubConnection?: signalR.HubConnection;

  private notificationsSubject = new BehaviorSubject<AppNotification[]>([]);
  notifications$ = this.notificationsSubject.asObservable();

  private unreadCountSubject = new BehaviorSubject<number>(0);
  unreadCount$ = this.unreadCountSubject.asObservable();

  async start(): Promise<void> {
    if (this.hubConnection) {
      return; // already connected
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
   .withUrl(`${environment.hubUrl}/hubs/notifications`, {
  accessTokenFactory: () => this.authService.getAccessToken() ?? ''
})
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveNotification', (notification: AppNotification) => {
      const current = this.notificationsSubject.value;
      this.notificationsSubject.next([notification, ...current]);
      this.unreadCountSubject.next(this.unreadCountSubject.value + 1);
    });

    try {
      await this.hubConnection.start();
    } catch (err) {
      console.error('SignalR connection failed:', err);
    }

    await this.loadInitialState();
  }

  async stop(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.hubConnection = undefined;
    }
    this.notificationsSubject.next([]);
    this.unreadCountSubject.next(0);
  }

private async loadInitialState(): Promise<void> {
  this.http.get<AppNotification[]>(`${environment.apiUrl}/Notifications`)
    .subscribe(list => this.notificationsSubject.next(list));

  this.http.get<{ unreadCount: number }>(`${environment.apiUrl}/Notifications/unread-count`)
    .subscribe(res => this.unreadCountSubject.next(res.unreadCount));
}

  markAsRead(id: number): void {
     this.http.patch(`${environment.apiUrl}/Notifications/${id}/read`, {})
      .subscribe(() => {
        const updated = this.notificationsSubject.value.map(n =>
          n.id === id ? { ...n, isRead: true } : n
        );
        this.notificationsSubject.next(updated);
        this.unreadCountSubject.next(Math.max(0, this.unreadCountSubject.value - 1));
      });
  }

  markAllAsRead(): void {
    this.http.patch(`${environment.apiUrl}/Notifications/read-all`, {})
      .subscribe(() => {
        const updated = this.notificationsSubject.value.map(n => ({ ...n, isRead: true }));
        this.notificationsSubject.next(updated);
        this.unreadCountSubject.next(0);
      });
  }
}