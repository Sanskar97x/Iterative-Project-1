using Microsoft.AspNetCore.Http;
using IterativeProject1.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace IterativeProject1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly IWebHostEnvironment _environment;

        public AuthController(IAuthService service, IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }
        private CookieOptions GetRefreshCookieOptions()
        {

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = _environment.IsDevelopment()
                    ? SameSiteMode.None
                    : SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var user = await _service.Register(dto);
            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login (LoginDto dto)
        {
            var response = await _service.Login(dto);

            Response.Cookies.Append(
                "refreshToken", response.RefreshToken,
                GetRefreshCookieOptions());
            response.RefreshToken = string.Empty;
            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            try
            {
                var response = await _service.RefreshToken(refreshToken);

                Response.Cookies.Append(
                    "refreshToken", response.RefreshToken,
                    GetRefreshCookieOptions());

                response.RefreshToken = string.Empty;
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _service.RevokeRefreshToken(refreshToken);
                Response.Cookies.Delete("refreshToken");
            }

            return NoContent();
        }

    }
}
