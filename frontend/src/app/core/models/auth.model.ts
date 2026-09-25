import { UserDetailDto, PreferredCalendar } from './user.model';

export interface LoginDto {
  identifier: string;
  password: string;
}

export interface RegisterUserDto {
  username: string;
  email: string;
  password: string;
  fullName: string;
  profession?: string;
  preferredCalendar: PreferredCalendar;
}

export interface LoginResponseDto {
  user: UserDetailDto;
  message: string;
  accessToken: string;
  refreshToken: string; // blanked out by the backend after the cookie is set
}

export interface RefreshTokenResponseDto {
  accessToken: string;
  refreshToken: string;
}