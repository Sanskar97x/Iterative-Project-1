using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class RescheduleBookingDto
    {
        public DateTime NewStart { get; set; }
        public DateTime NewEnd { get; set; }
    }
}
