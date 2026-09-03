using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IterativeProject1.DTOs
{
    public class LoginDto
    {
        [Required]

        public string Identifier { get; set; } = string.Empty;

        [Required]

        public string Password { get; set; } = string.Empty;
    }
}
