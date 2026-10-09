using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace IterativeProject1.Hubs
{
    [Authorize(Roles = "Admin")]
    public class NotificationHub: Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if(!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }
        public class NotificationDto
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string Message { get; set; }
            public int? RelatedBookingId { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // SignalR automatically removes the connection from groups on disconnect,
            // so usually you don't need to manually remove it here. Just call base.
            await base.OnDisconnectedAsync(exception);
        }
    }
}
