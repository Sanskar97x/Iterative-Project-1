import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { BookingService } from '../../../core/services/booking';
import { AvailabilityService } from '../../../core/services/availability';
import { BookingResponseDto } from '../../../core/models/booking.model';
import { SlotStatusDto } from '../../../core/models/availability.model';

interface CalendarCell {
  date: Date | null;
  key: string;
  isPast: boolean;
  isToday: boolean;
}

function toDateKey(d: Date): string {
  const y = d.getFullYear();
  const m = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${y}-${m}-${day}`;
}

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-bookings.html',
})
export class MyBookings implements OnInit {
  private bookingService = inject(BookingService);
  private availabilityService = inject(AvailabilityService);

  readonly bookings = signal<BookingResponseDto[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly actioningId = signal<number | null>(null);

  readonly reschedulingBooking = signal<BookingResponseDto | null>(null);
  readonly today = new Date();
  readonly viewMonth = signal(new Date(this.today.getFullYear(), this.today.getMonth(), 1));
  readonly availableDateKeys = signal<Set<string>>(new Set());
  readonly calendarLoading = signal(false);
  readonly selectedDateKey = signal<string | null>(null);
  readonly daySlots = signal<SlotStatusDto[]>([]);
  readonly slotsLoading = signal(false);
  readonly rescheduling = signal(false);
  readonly popupMessage = signal<string | null>(null);

  readonly monthLabel = computed(() =>
    this.viewMonth().toLocaleDateString(undefined, { month: 'long', year: 'numeric' })
  );

  readonly calendarCells = computed<CalendarCell[]>(() => {
    const month = this.viewMonth();
    const year = month.getFullYear();
    const monthIndex = month.getMonth();
    const firstDay = new Date(year, monthIndex, 1);
    const daysInMonth = new Date(year, monthIndex + 1, 0).getDate();
    const leadingBlanks = firstDay.getDay();

    const cells: CalendarCell[] = [];
    for (let i = 0; i < leadingBlanks; i++) {
      cells.push({ date: null, key: `blank-${i}`, isPast: false, isToday: false });
    }
    for (let d = 1; d <= daysInMonth; d++) {
      const date = new Date(year, monthIndex, d);
      const key = toDateKey(date);
      cells.push({
        date,
        key,
        isPast: date < new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate()),
        isToday: key === toDateKey(this.today),
      });
    }
    return cells;
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.bookingService.getMine().subscribe({
      next: (bookings) => {
        this.bookings.set(bookings);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load your bookings.');
        this.loading.set(false);
      },
    });
  }

  cancel(id: number): void {
    this.actioningId.set(id);
    this.bookingService.cancel(id).subscribe({
      next: () => {
        this.actioningId.set(null);
        this.load();
      },
      error: () => {
        this.actioningId.set(null);
        this.errorMessage.set('Could not cancel that booking.');
      },
    });
  }

  startReschedule(booking: BookingResponseDto): void {
    this.reschedulingBooking.set(booking);
    this.viewMonth.set(new Date(this.today.getFullYear(), this.today.getMonth(), 1));
    this.selectedDateKey.set(null);
    this.daySlots.set([]);
    this.loadMonthAvailability();
  }

  cancelReschedule(): void {
    this.reschedulingBooking.set(null);
    this.selectedDateKey.set(null);
    this.daySlots.set([]);
  }

  changeMonth(offset: number): void {
    const current = this.viewMonth();
    this.viewMonth.set(new Date(current.getFullYear(), current.getMonth() + offset, 1));
    this.selectedDateKey.set(null);
    this.daySlots.set([]);
    this.loadMonthAvailability();
  }

  loadMonthAvailability(): void {
    const booking = this.reschedulingBooking();
    if (!booking) return;

    this.calendarLoading.set(true);
    const month = this.viewMonth();
    const year = month.getFullYear();
    const monthIndex = month.getMonth();
    const daysInMonth = new Date(year, monthIndex + 1, 0).getDate();

        const startOfToday = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate());
    const dateKeys: string[] = [];
    for (let d = 1; d <= daysInMonth; d++) {
      const candidate = new Date(year, monthIndex, d);
      if (candidate >= startOfToday) {
        dateKeys.push(toDateKey(candidate));
      }
    }

    const requests = dateKeys.map((key) =>
      this.availabilityService.getOpenSlots(booking.providerId, key)
    );

    forkJoin(requests).subscribe({
      next: (results) => {
        const available = new Set<string>();
        results.forEach((res, i) => {
          if (res.totalOpenSlots > 0) {
            available.add(dateKeys[i]);
          }
        });
        this.availableDateKeys.set(available);
        this.calendarLoading.set(false);
      },
      error: () => {
        this.calendarLoading.set(false);
      },
    });
  }

  selectDate(cell: CalendarCell): void {
    if (!cell.date) return;

    if (cell.isPast) {
      this.popupMessage.set('That date has already passed.');
      return;
    }

    if (!this.availableDateKeys().has(cell.key)) {
      this.popupMessage.set('No availability on this date.');
      return;
    }

    this.selectedDateKey.set(cell.key);
    this.loadDaySlots();
  }

  loadDaySlots(): void {
    const booking = this.reschedulingBooking();
    const key = this.selectedDateKey();
    if (!booking || !key) return;

    this.slotsLoading.set(true);
    this.daySlots.set([]);

    this.availabilityService.getSlotStatuses(booking.providerId, key).subscribe({
      next: (res) => {
        this.daySlots.set(res.slots);
        this.slotsLoading.set(false);
      },
      error: () => {
        this.popupMessage.set('Could not load times for that date.');
        this.slotsLoading.set(false);
      },
    });
  }

  pickSlot(slot: SlotStatusDto): void {
    if (slot.status === 'BookedByYou') {
      this.popupMessage.set('You already have a booking at this time.');
      return;
    }
    if (slot.status === 'Booked') {
      this.popupMessage.set('This time is already booked by another client.');
      return;
    }

    const booking = this.reschedulingBooking();
    if (!booking) return;

    this.rescheduling.set(true);

    this.bookingService
      .reschedule(booking.id, { newStart: slot.slotStart, newEnd: slot.slotEnd })
      .subscribe({
        next: () => {
          this.rescheduling.set(false);
          this.cancelReschedule();
          this.load();
        },
        error: (err) => {
          this.rescheduling.set(false);
          this.popupMessage.set(
            err.status === 409
              ? 'That time was just taken — please pick another.'
              : 'Could not reschedule that booking.'
          );
          this.loadDaySlots();
        },
      });
  }

  closePopup(): void {
    this.popupMessage.set(null);
  }
}