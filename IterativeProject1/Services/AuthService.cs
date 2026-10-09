using System;
using System.Collections.Generic;
using System.Linq;
using IterativeProject1.Exceptions;
using System.Threading.Tasks;
using IterativeProject1.IServices;
using IterativeProject1.DTOs;
using IterativeProject1.Repositories;
using IterativeProject1.Models;
using BCrypt.Net;


namespace IterativeProject1.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshRepo;

        public AuthService(IUserRepository repo, IJwtService JwtService, IRefreshTokenRepository refreshRepo)
        {
            _repo = repo;
            _jwtService = JwtService;
            _refreshRepo = refreshRepo;
        }

        public async Task<RefreshTokenResponseDto> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedAccessException("No refresh token provided.");

            var stored = await _refreshRepo.GetByToken(refreshToken);

            if (stored == null || stored.RevokedAt != null || stored.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var user = await _repo.GetById(stored.UserId); // <-- need your actual method name here

            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            await _refreshRepo.Revoke(stored.RefreshTokenId, DateTime.UtcNow);

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            await _refreshRepo.Create(new RefreshToken
            {
                UserId = user.UserId,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task RevokeRefreshToken(string refreshToken)
        {
            await _refreshRepo.RevokeByToken(refreshToken, DateTime.UtcNow);
        }

        public async Task<UserDetailDto> Register(RegisterUserDto dto)
        {
            ValidatePassword(dto.Password);
            ValidatePreferredCalendar(dto.PreferredCalendar);
            
            if (await _repo.UsernameExists(dto.Username))
            {
                throw new UsernameAlreadyExistException();
            }

            if (await _repo.EmailExists(dto.Email))
            {
                throw new EmailAlreadyExistsException();            
            }

            var verificationToken = _jwtService.GenerateEmailVerificationToken();
            var expiresAt = DateTime.UtcNow.AddHours(24);                                                        

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Profession = dto.Profession,
                PreferredCalendar = dto.PreferredCalendar,
                Role = "User",
                AuthProvider = "Local",
                IsEmailVerified = false,
                EmailVerificationToken = verificationToken,
                EmailVerificationExpiresAt = expiresAt
            };

            int newUserId = await _repo.Create(user);
            //await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, verificationToken)


            return new UserDetailDto
           {
                UserId = newUserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Profession = user.Profession,
                PreferredCalendar = user.PreferredCalendar,
                Role = user.Role,
                AuthProvider = user.AuthProvider,
                HasPassword = !string.IsNullOrEmpty(user.Password)
           };
        }

        private void ValidatePassword(string password)
        {
            if (!password.Any(char.IsUpper))
            {
                throw new InvalidPasswordException("Password must contain at least one uppercase letter.");
            }

            if (!password.Any(char.IsLower))
            {
                throw new InvalidPasswordException("Password must contain at least one lowercase letter.");
            }

            if (!password.Any(char.IsDigit))
            {
                throw new InvalidPasswordException("Password must contain at least one number.");
            }
        }

        private void ValidatePreferredCalendar(string preferredCalendar)
        {
            if (!preferredCalendar.Equals("English", StringComparison.OrdinalIgnoreCase) && !preferredCalendar.Equals("Nepali", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidPreferredCalendarException();
            }
        }



        public async Task<LoginResponseDto> Login(LoginDto dto)
        {
            var user = await _repo.GetByEmailOrUserName(dto.Identifier);

            if (user == null)
            {
                throw new InvalidCredentialsException("Invalid username/email or password.");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                throw new InvalidCredentialsException("Please continue with Google or set a password first.");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                throw new InvalidCredentialsException("Invalid username/email or password.");
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshRepo.Create(refreshTokenEntity);


            return new LoginResponseDto
            {
                User = new UserDetailDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Profession = user.Profession,
                    PreferredCalendar = user.PreferredCalendar,
                    Role = user.Role,
                    AuthProvider = user.AuthProvider,
                    HasPassword = !string.IsNullOrEmpty(user.Password)
                },
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}


