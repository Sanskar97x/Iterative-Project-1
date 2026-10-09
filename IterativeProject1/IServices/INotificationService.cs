using IterativeProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IterativeProject1.IServices
{
    public interface INotificationService
    {
        Task<int> Create(Notification notification);
        Task<IEnumerable<Notification>> GetByUserId(int userId, bool unreadOnly);
        Task<bool> MarkAsRead(int notificationId, int requestingUserId);
        Task<bool> MarkAllAsRead(int userId);
        Task<int> GetUnreadCount(int userId);
        Task CreateBookingCancelledNotification(Booking booking);
        Task<bool> MarkAsUnread(int notificationId, int requestingUserId);

    }
}
