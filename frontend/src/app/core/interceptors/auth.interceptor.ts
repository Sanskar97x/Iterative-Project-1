import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getAccessToken();

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` }, withCredentials: true })
    : req.clone({ withCredentials: true });

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Don't try to refresh on the refresh/login/register calls themselves
      const isAuthCall = req.url.includes('/Auth/');

      if (error.status === 401 && !isAuthCall) {
        return authService.refresh().pipe(
          switchMap((res) => {
            if (!res) {
              return throwError(() => error);
            }
            const retryReq = req.clone({
              setHeaders: { Authorization: `Bearer ${res.accessToken}` },
              withCredentials: true
            });
            return next(retryReq);
          })
        );
      }

      return throwError(() => error);
    })
  );
};