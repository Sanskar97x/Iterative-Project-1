using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.DTOs;
using IterativeProject1.Models;

namespace IterativeProject1.IServices
{
    public interface INotificationPublisher
    { 
       Task PublishToUser(int userId, NotificationDto notification);
    }
}
