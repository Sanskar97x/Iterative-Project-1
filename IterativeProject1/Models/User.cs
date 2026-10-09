using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.Enums;

namespace IterativeProject1.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Profession { get; set; }
        public string PreferredCalendar { get; set; } = string.Empty;
        public string Role { get; set; } = "Client"; // stored as varchar in DB
        public string AuthProvider { get; set; }
        public bool IsEmailVerified { get; set; }
        public string EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationExpiresAt { get; set; }
        public UserRole RoleEnum => Enum.Parse<UserRole>(Role);
    }
}
