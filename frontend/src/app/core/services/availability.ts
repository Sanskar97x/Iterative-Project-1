import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AvailableSlotsResponseDto, CreateAvailabilityDto, DaySlotStatusResponseDto } from '../models/availability.model';

@Injectable({ providedIn: 'root' })
export class AvailabilityService {
  private readonly baseUrl = `${environment.apiUrl}/Availability`;

  constructor(private http: HttpClient) {}

  // Admin only — backend enforces the role
  create(dto: CreateAvailabilityDto): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.baseUrl}/Create`, dto);
  }

  // Admin only — lists the logged-in admin's own availability windows
  getMine(): Observable<CreateAvailabilityDto[]> {
    return this.http.get<CreateAvailabilityDto[]>(`${this.baseUrl}/mine`);
  }

  getOpenSlots(
    providerId: number,
    date: string,
    slotDurationMinutes = 30
  ): Observable<AvailableSlotsResponseDto> {
    const params = new HttpParams()
      .set('providerId', providerId)
      .set('date', date)
      .set('slotDurationMinutes', slotDurationMinutes);

    return this.http.get<AvailableSlotsResponseDto>(`${this.baseUrl}/slots`, { params });
  }

  getSlotStatuses(
    providerId: number,
    date: string,
    slotDurationMinutes = 30
  ): Observable<DaySlotStatusResponseDto>{
    const params = new HttpParams()
    .set('providerId', providerId)
    .set('date',date)
    .set('slotDurationMinutes', slotDurationMinutes);

    return this.http.get<DaySlotStatusResponseDto>(`${this.baseUrl}/slot-statuses`,{params})
  }

  // Admin only. `id` goes as a query param, matching the controller's binding.
  delete(id: number): Observable<void> {
    const params = new HttpParams().set('id', id);
    return this.http.delete<void>(`${this.baseUrl}/Delete`, { params });
  }
}