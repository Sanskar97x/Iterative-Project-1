using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.Models;

namespace IterativeProject1.IRepositories
{
    public interface INotificationRepository
    {
        Task<int> Create(Notification notification);
        Task<IEnumerable<Notification>> GetByUserId(int userId, bool unreadOnly);
        Task<bool> MarkAsRead(int notificationId, int userId);
        Task<bool> MarkAllAsRead(int userId);
        Task<int> GetUnreadCount(int userId);
        Task<bool> MarkAsUnread(int notificationId, int userId);
    }
}
