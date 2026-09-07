using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.Models;

namespace IterativeProject1.Helpers
{
    public class TimeSlotHelper
    {
        public static List<(DateTime Start, DateTime End)> GenerateOpenSlots(DateTime date, IEnumerable<Availability> availabilities, IEnumerable<Booking> existingBookings, int slotDurationMinutes)       
        {
            var openSlots = new List<(DateTime, DateTime)>();

            foreach(var availability in availabilities)
            {
                var windowStart = date.Date + availability.StartTime;
                var windowEnd = date.Date + availability.EndTime;

                var cursor = windowStart;
                while (cursor.AddMinutes(slotDurationMinutes) <= windowEnd)
                {
                    var slotEnd = cursor.AddMinutes(slotDurationMinutes);

                    bool overlapsBooking = existingBookings.Any(b => cursor < b.EndTime && slotEnd > b.StartTime);

                    if (!overlapsBooking)
                    {
                        openSlots.Add((cursor, slotEnd));
                    }
                    cursor = slotEnd;
                }
            }
            return openSlots;
        }

    }
}
