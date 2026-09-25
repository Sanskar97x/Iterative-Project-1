import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AvailabilityService } from '../../../core/services/availability';
import { CreateAvailabilityDto } from '../../../core/models/availability.model';

@Component({
  selector: 'app-manage-availability',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './manage-availability.html',
})
export class ManageAvailability implements OnInit {
  private fb = inject(FormBuilder);
  private availabilityService = inject(AvailabilityService);

  readonly windows = signal<CreateAvailabilityDto[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly errorMessage = signal<string | null>(null);

  // 'recurring' uses dayOfWeek, 'specific' uses a one-off date
  form = this.fb.group({
    mode: ['recurring' as 'recurring' | 'specific', Validators.required],
    dayOfWeek: [1, Validators.required],
    specificDate: [''],
    startTime: ['09:00', Validators.required],
    endTime: ['17:00', Validators.required],
  });

  readonly days = [
    { value: 0, label: 'Sunday' },
    { value: 1, label: 'Monday' },
    { value: 2, label: 'Tuesday' },
    { value: 3, label: 'Wednesday' },
    { value: 4, label: 'Thursday' },
    { value: 5, label: 'Friday' },
    { value: 6, label: 'Saturday' },
  ];

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.availabilityService.getMine().subscribe({
      next: (windows) => {
        this.windows.set(windows);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load your availability windows.');
        this.loading.set(false);
      },
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const isRecurring = this.form.value.mode === 'recurring';

    const dto: CreateAvailabilityDto = {
      dayOfWeek: isRecurring ? this.form.value.dayOfWeek! : undefined,
      specificDate: !isRecurring ? this.form.value.specificDate! : undefined,
      startTime: `${this.form.value.startTime}:00`,
      endTime: `${this.form.value.endTime}:00`,
    };

    this.saving.set(true);
    this.errorMessage.set(null);

    this.availabilityService.create(dto).subscribe({
      next: () => {
        this.saving.set(false);
        this.load();
      },
      error: () => {
        this.saving.set(false);
        this.errorMessage.set('Could not save that availability window.');
      },
    });
  }

  remove(id: number | undefined): void {
    if (!id) return;
    this.availabilityService.delete(id).subscribe({
      next: () => this.load(),
      error: () => this.errorMessage.set('Could not delete that window.'),
    });
  }

  dayLabel(day: number | undefined): string {
    return this.days.find((d) => d.value === day)?.label ?? '';
  }
}