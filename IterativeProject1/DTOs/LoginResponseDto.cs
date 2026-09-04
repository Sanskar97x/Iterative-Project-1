using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class LoginResponseDto 
    {
        public UserDetailDto User { get; set; } = new UserDetailDto();
        public string Message { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
