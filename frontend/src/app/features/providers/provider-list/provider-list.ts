import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UserService } from '../../../core/services/user';
import { UserSummaryDto } from '../../../core/models/user-summary.model';

@Component({
  selector: 'app-provider-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './provider-list.html',
})
export class ProviderList implements OnInit {
  private userService = inject(UserService);

  readonly providers = signal<UserSummaryDto[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.userService.getByRole('Admin').subscribe({
      next: (providers) => {
        this.providers.set(providers);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load providers. Please try again.');
        this.loading.set(false);
      },
    });
  }
}