using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public int ? RelatedBookingId {get; set;}
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
