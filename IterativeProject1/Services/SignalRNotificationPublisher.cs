using IterativeProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.IServices;
using Microsoft.AspNetCore.SignalR;
using IterativeProject1.Hubs;
using IterativeProject1.DTOs;

namespace IterativeProject1.Services
{
    public class SignalRNotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationPublisher(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public Task PublishToUser(int userId, NotificationDto notification)
        {
            return _hubContext.Clients
                 .Group(userId.ToString())
                 .SendAsync("ReceiveNotification", notification);
        }
    }
}
