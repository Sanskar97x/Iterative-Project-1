using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class AvailabilitySlotDto
    {
        public int ProviderId { get; set; }
        public DateTime SlotStart { get; set; }
        public DateTime SlotEnd { get; set; }
    }
}

// This is what the client sees when browsing open slots — not raw Availability rows, but computed bookable windows.
