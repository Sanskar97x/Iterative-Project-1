using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class AvailableSlotsResponseDto
    {
        public int ProviderId { get; set; }
        public DateTime Date { get; set; }
        public int SlotDurationMinutes { get; set; }
        public int InactiveAvailabilityWindowsFound { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalAvailabilityWindowsFound { get; set; }  // debug: how many Availability rows matched
        public int TotalOpenSlots { get; set; }
        public List<AvailabilitySlotDto> Slots { get; set; } = new List<AvailabilitySlotDto>();

    }
}
