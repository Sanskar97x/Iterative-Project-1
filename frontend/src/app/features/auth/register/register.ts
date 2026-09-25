import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  form = this.fb.group({
    username: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    fullName: ['', Validators.required],
    profession: [''],
    preferredCalendar: ['English', Validators.required],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);

    // Password must contain an uppercase letter, a lowercase letter, and a
    // digit, and preferredCalendar must be "English" or "Nepali" — both
    // enforced server-side by AuthService.Register.
    this.authService
      .register({
        username: this.form.value.username!,
        email: this.form.value.email!,
        password: this.form.value.password!,
        fullName: this.form.value.fullName!,
        profession: this.form.value.profession || undefined,
        preferredCalendar: this.form.value.preferredCalendar as 'English' | 'Nepali',
      })
      .subscribe({
        next: () => this.router.navigate(['/login']),
        error: (err) => {
          this.loading.set(false);
          this.errorMessage.set(
            err.error?.message ?? 'Something went wrong. Please check your details and try again.'
          );
        },
      });
  }
}