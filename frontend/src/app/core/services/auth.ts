import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginDto, LoginResponseDto, RegisterUserDto, RefreshTokenResponseDto } from '../models/auth.model';
import { UserDetailDto, UserRole } from '../models/user.model';

function decodeJwtUser(token: string): UserDetailDto | null {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return {
      userId: Number(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']),
      username: payload['username'],
      email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
      fullName: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] as UserRole,
      preferredCalendar: 'English', // not carried in the JWT — refreshed properly on next real login
      authProvider: 'Local',
      hasPassword: true,
    };
  } catch {
    return null;
  }
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = `${environment.apiUrl}/Auth`;

  private accessToken = signal<string | null>(null);
  private currentUser = signal<UserDetailDto | null>(null);

  readonly user = computed(() => this.currentUser());
  readonly isLoggedIn = computed(() => !!this.accessToken());
  readonly role = computed(() => this.currentUser()?.role ?? null);

  constructor(private http: HttpClient, private router: Router) {}

  getAccessToken(): string | null {
    return this.accessToken();
  }

  register(dto: RegisterUserDto): Observable<UserDetailDto> {
    return this.http.post<UserDetailDto>(`${this.baseUrl}/register`, dto);
  }

  login(dto: LoginDto): Observable<LoginResponseDto> {
    return this.http
      .post<LoginResponseDto>(`${this.baseUrl}/Login`, dto, { withCredentials: true })
      .pipe(
        tap((res) => {
          this.accessToken.set(res.accessToken);
          this.currentUser.set(res.user);
        })
      );
  }

  refresh(): Observable<RefreshTokenResponseDto | null> {
    return this.http
      .post<RefreshTokenResponseDto>(`${this.baseUrl}/refresh`, {}, { withCredentials: true })
      .pipe(
        tap((res) => {
          this.accessToken.set(res.accessToken);
          // The refresh endpoint doesn't return user details — reconstruct
          // enough of them from the JWT's own claims so a hard reload
          // still shows the right name/role without another round trip.
          this.currentUser.set(decodeJwtUser(res.accessToken));
        }),
        catchError(() => {
          this.clearSession();
          return of(null);
        })
      );
  }

  logout(): void {
    this.http
      .post(`${this.baseUrl}/logout`, {}, { withCredentials: true })
      .subscribe({
        complete: () => {
          this.clearSession();
          this.router.navigate(['/login']);
        }
      });
  }

  private clearSession(): void {
    this.accessToken.set(null);
    this.currentUser.set(null);
  }
}