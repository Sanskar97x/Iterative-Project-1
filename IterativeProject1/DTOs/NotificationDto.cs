using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public int? RelatedBookingId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}