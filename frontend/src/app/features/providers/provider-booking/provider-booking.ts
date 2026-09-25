import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AvailabilityService } from '../../../core/services/availability';
import { BookingService } from '../../../core/services/booking';
import { AvailabilitySlotDto } from '../../../core/models/availability.model';

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
  selector: 'app-provider-booking',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './provider-booking.html',
})
export class ProviderBooking implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private availabilityService = inject(AvailabilityService);
  private bookingService = inject(BookingService);

  readonly providerId = Number(this.route.snapshot.paramMap.get('id'));

  readonly today = new Date();
  readonly viewMonth = signal(new Date(this.today.getFullYear(), this.today.getMonth(), 1));
  readonly selectedDateKey = signal<string | null>(null);

  readonly availableDateKeys = signal<Set<string>>(new Set());
  readonly calendarLoading = signal(false);

  readonly slots = signal<AvailabilitySlotDto[]>([]);
  readonly message = signal<string>('');
  readonly slotsLoading = signal(false);
  readonly booking = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly bookedSlot = signal<AvailabilitySlotDto | null>(null);

  // Popup shown when clicking a non-available date
  readonly unavailablePopup = signal<string | null>(null);

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
    this.loadMonthAvailability();
  }

  changeMonth(offset: number): void {
    const current = this.viewMonth();
    this.viewMonth.set(new Date(current.getFullYear(), current.getMonth() + offset, 1));
    this.selectedDateKey.set(null);
    this.slots.set([]);
    this.loadMonthAvailability();
  }

  loadMonthAvailability(): void {
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
      this.availabilityService.getOpenSlots(this.providerId, key)
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
      this.unavailablePopup.set('That date has already passed.');
      return;
    }

    if (!this.availableDateKeys().has(cell.key)) {
      this.unavailablePopup.set('No availability on this date.');
      return;
    }

    this.selectedDateKey.set(cell.key);
    this.loadSlotsForSelected();
  }

  closePopup(): void {
    this.unavailablePopup.set(null);
  }

  loadSlotsForSelected(): void {
    const key = this.selectedDateKey();
    if (!key) return;

    this.slotsLoading.set(true);
    this.errorMessage.set(null);
    this.slots.set([]);

    this.availabilityService.getOpenSlots(this.providerId, key).subscribe({
      next: (res) => {
        this.slots.set(res.slots);
        this.message.set(res.message);
        this.slotsLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load availability. Please try again.');
        this.slotsLoading.set(false);
      },
    });
  }

  bookSlot(slot: AvailabilitySlotDto): void {
    this.booking.set(true);
    this.errorMessage.set(null);

    this.bookingService
      .create({
        providerId: this.providerId,
        startTime: slot.slotStart,
        endTime: slot.slotEnd,
      })
      .subscribe({
        next: () => {
          this.booking.set(false);
          this.bookedSlot.set(slot);
        },
        error: (err) => {
          this.booking.set(false);
          this.errorMessage.set(
            err.status === 409
              ? 'That slot was just taken — please pick another.'
              : 'Something went wrong booking that slot.'
          );
          this.loadSlotsForSelected();
          this.loadMonthAvailability();
        },
      });
  }

  goToMyBookings(): void {
    this.router.navigate(['/bookings/mine']);
  }
}