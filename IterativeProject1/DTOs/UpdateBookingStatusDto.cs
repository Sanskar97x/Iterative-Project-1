using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class UpdateBookingStatusDto
    {
        public string Status { get; set; } = string.Empty; // validate against BookingStatus enum values
    }
}
