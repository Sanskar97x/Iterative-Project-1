import { inject } from '@angular/core';
import { provideAppInitializer } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AuthService } from './services/auth';

// Silently attempts to restore the session from the HttpOnly refresh-token
// cookie when the app boots (e.g. on a hard page reload). If the cookie is
// missing or expired, AuthService.refresh() already clears the session and
// resolves without throwing, so the app just lands the user on the login flow.
export const appInitializerProvider = provideAppInitializer(() => {
  const authService = inject(AuthService);
  return firstValueFrom(authService.refresh());
});