import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { BookingService } from '../../../core/services/booking';
import { BookingResponseDto, BookingStatus } from '../../../core/models/booking.model';

@Component({
  selector: 'app-all-bookings',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './all-bookings.html',
})
export class AllBookings implements OnInit {
  private bookingService = inject(BookingService);

  readonly bookings = signal<BookingResponseDto[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly updatingId = signal<number | null>(null);

  readonly statuses: BookingStatus[] = ['Pending', 'Confirmed', 'Cancelled', 'Completed'];

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.bookingService.getAll().subscribe({
      next: (bookings) => {
        this.bookings.set(bookings);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load bookings.');
        this.loading.set(false);
      },
    });
  }

  updateStatus(id: number, status: BookingStatus): void {
    this.updatingId.set(id);
    this.bookingService.updateStatus(id, { status }).subscribe({
      next: () => {
        this.updatingId.set(null);
        this.load();
      },
      error: () => {
        this.updatingId.set(null);
        this.errorMessage.set('Could not update that booking.');
      },
    });
  }
}