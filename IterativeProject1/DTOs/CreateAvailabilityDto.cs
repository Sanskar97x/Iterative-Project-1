using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class CreateAvailabilityDto
    {
        public int? Id { get; set; }
        public byte? DayOfWeek { get; set; }
        public DateTime? SpecificDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
