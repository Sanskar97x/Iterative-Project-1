using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class SlotStatusDto
    {
        public DateTime SlotStart { get; set; }
        public DateTime SlotEnd { get; set; }
        public string Status { get; set; } = "Open"; // "Open" | "BookedByYou" | "Booked"
    }
}
