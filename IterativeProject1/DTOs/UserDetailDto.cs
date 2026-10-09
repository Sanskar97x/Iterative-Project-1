using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class UserDetailDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public string AuthProvider { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Profession { get; set; }

        public string PreferredCalendar { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        public bool HasPassword { get; set; }
    }
}
