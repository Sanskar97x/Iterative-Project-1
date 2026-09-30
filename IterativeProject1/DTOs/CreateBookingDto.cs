using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class CreateBookingDto
    {
        public int ProviderId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Notes { get; set; }
    }
}
