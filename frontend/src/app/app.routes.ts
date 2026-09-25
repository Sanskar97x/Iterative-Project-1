import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';


export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register').then((m) => m.RegisterComponent),
  },

    {
    path: 'providers',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/providers/provider-list/provider-list').then((m) => m.ProviderList),
  },
  {
    path: 'providers/:id/book',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/providers/provider-booking/provider-booking').then((m) => m.ProviderBooking),
  },

    {
    path: 'bookings/mine',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/bookings/my-bookings/my-bookings').then((m) => m.MyBookings),
  },

  {
    path: 'admin/availability',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' },
    loadComponent: () =>
      import('./features/admin/manage-availability/manage-availability').then((m) => m.ManageAvailability),
  },

  {
    path: 'admin/bookings',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Admin' },
    loadComponent: () =>
      import('./features/admin/all-bookings/all-bookings').then((m) => m.AllBookings),
  },

  {
  path: '',
  pathMatch: 'full',
  canActivate: [authGuard],
  loadComponent: () =>
    import('./features/dashboard/dashboard/dashboard').then((m) => m.Dashboard),
},

  { path: '**', redirectTo: '' },
];