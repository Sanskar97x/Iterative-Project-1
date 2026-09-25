import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  BookingResponseDto,
  CreateBookingDto,
  RescheduleBookingDto,
  UpdateBookingStatusDto,
} from '../models/booking.model';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private readonly baseUrl = `${environment.apiUrl}/Booking`;

  constructor(private http: HttpClient) {}

  getById(id: number): Observable<BookingResponseDto> {
    return this.http.get<BookingResponseDto>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateBookingDto): Observable<BookingResponseDto> {
    return this.http.post<BookingResponseDto>(`${this.baseUrl}/CreateBooking`, dto);
  }

  // Bookings for the currently logged-in client
  getMine(): Observable<BookingResponseDto[]> {
    return this.http.get<BookingResponseDto[]>(`${this.baseUrl}/mine`);
  }

  // Admin only — backend enforces the role
  getAll(): Observable<BookingResponseDto[]> {
    return this.http.get<BookingResponseDto[]>(this.baseUrl);
  }

  cancel(id: number): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/cancel`, {});
  }

  // Note: the backend cancels the original booking and creates a new one on
  // reschedule, so the returned booking has a different id than the one you
  // called this with.
  reschedule(id: number, dto: RescheduleBookingDto): Observable<BookingResponseDto> {
    return this.http.patch<BookingResponseDto>(`${this.baseUrl}/${id}/reschedule`, dto);
  }

  // Admin only
  updateStatus(id: number, dto: UpdateBookingStatusDto): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/status`, dto);
  }
}