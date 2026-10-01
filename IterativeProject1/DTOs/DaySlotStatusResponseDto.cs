using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class DaySlotStatusResponseDto
    {
        public int ProviderId { get; set; }
        public DateTime Date { get; set; }
        public int SlotDurationMinutes { get; set; }
        public List<SlotStatusDto> Slots { get; set; } = new List<SlotStatusDto>();
    }
}
