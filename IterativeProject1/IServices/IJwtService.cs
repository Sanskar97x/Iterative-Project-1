using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.Models;

namespace IterativeProject1.IServices
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string GenerateEmailVerificationToken();
         string GenerateSecureToken();

        //string GeneratePasswordResetToken();
    }
}
