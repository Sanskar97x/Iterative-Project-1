export type BookingStatus = 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed';

export interface BookingResponseDto {
  id: number;
  clientId: number;
  providerId: number;
  startTime: string; // ISO date string
  endTime: string;
  status: BookingStatus;
  notes?: string;
}

export interface CreateBookingDto {
  providerId: number;
  startTime: string; // ISO date string
  endTime: string;
  notes?: string;
}

export interface RescheduleBookingDto {
  newStart: string; // ISO date string
  newEnd: string;
}

export interface UpdateBookingStatusDto {
  status: BookingStatus;
}