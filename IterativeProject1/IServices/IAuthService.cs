using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.DTOs;

namespace IterativeProject1.IServices
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginDto dto);
        Task<UserDetailDto> Register(RegisterUserDto dto);
        Task<RefreshTokenResponseDto> RefreshToken(string refreshToken); // new
        Task RevokeRefreshToken(string refreshToken); // new, for logout


    };
}
