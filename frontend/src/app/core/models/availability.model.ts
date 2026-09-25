export interface CreateAvailabilityDto {
  id?: number;
  dayOfWeek?: number;
  specificDate?: string;
  startTime: string;
  endTime: string;
  isActive?: boolean;
}

export interface AvailabilitySlotDto {
  providerId: number;
  slotStart: string;
  slotEnd: string;
}

export interface AvailableSlotsResponseDto {
  providerId: number;
  date: string;
  slotDurationMinutes: number;
  totalAvailabilityWindowsFound: number;
  inactiveAvailabilityWindowsFound: number;
  totalOpenSlots: number;
  message: string;
  slots: AvailabilitySlotDto[];
}

export type SlotStatus = 'Open' | 'BookedByYou' | 'Booked';

export interface SlotStatusDto {
  slotStart: string;
  slotEnd: string;
  status: SlotStatus;
}

export interface DaySlotStatusResponseDto {
  providerId: number;
  date: string;
  slotDurationMinutes: number;
  slots: SlotStatusDto[];
}