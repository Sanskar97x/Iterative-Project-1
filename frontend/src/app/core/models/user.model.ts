export type PreferredCalendar = 'English' | 'Nepali';
export type UserRole = 'Client' | 'Admin';

export interface UserDetailDto {
  userId: number;
  username: string;
  email: string;
  fullName: string;
  profession?: string;
  preferredCalendar: PreferredCalendar;
  role: UserRole;
  authProvider: string;
  hasPassword: boolean;
}