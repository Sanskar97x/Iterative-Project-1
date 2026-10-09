using IterativeProject1.IRepositories;
using IterativeProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IterativeProject1.Data;

namespace IterativeProject1.Repositories
{
    public class NotificationRepository : INotificationRepository { 

        private readonly DapperContext _context;

        public NotificationRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> Create(Notification notification)
        {
            var query = @" INSERT INTO [Notification] (UserId, Type, Message, IsRead, CreatedAt) VALUES 
                (@UserId, @Type, @Message, @IsRead, @CreatedAt);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using var connection = _context.CreateConnection();

            var newID = await connection.ExecuteScalarAsync<int>(query, new
            {
                notification.UserId,
                notification.Type,
                notification.Message,
                notification.IsRead,
                notification.CreatedAt
            });

            return newID;
        }

        public async Task<IEnumerable<Notification>> GetByUserId(int userId, bool unreadOnly)
        {
            var query = @"SELECT * FROM [Notification]
            WHERE UserId = @UserId" + (unreadOnly ? "AND IsRead =0" : " ") + @"ORDER BY CreatedAt DESC;";
            using var connection = _context.CreateConnection();
            var notifications = await connection.QueryAsync<Notification>(query, new { UserId = userId });
            return notifications;
        }

        public async Task<bool> MarkAsRead(int notificationId, int userId)
        {
            var query = @"UPDATE [Notification] 
                  SET IsRead = 1 
                  WHERE NotificationId = @NotificationId 
                    AND UserId = @UserId 
                    AND IsRead = 0;";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, new { NotificationId = notificationId, UserId = userId });
            return rowsAffected > 0;
        }
        public async Task<bool> MarkAllAsRead(int userId)
        {
            var query = @"UPDATE [Notification] SET IsRead = 1 WHERE UserId = @UserId AND IsRead = 0;";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, new { UserId = userId });
            return rowsAffected > 0;
        }
        public async Task<int> GetUnreadCount(int userId)
        {
            var query = @"SELECT COUNT(*) FROM [Notification] WHERE UserId = @UserId And IsRead = 0;";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(query, new { UserId = userId });
            return count;
        }

        public async Task<bool> MarkAsUnread (int notificationId, int userId)
        {
            var query = @"UPDATE [Notification] 
                  SET IsRead = 0
                  WHERE NotificationId = @NotificationId 
                    AND UserId = @UserId 
                    AND IsRead = 1;";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, new { NotificationId = notificationId, UserId = userId });
            return rowsAffected > 0; 
        }
    }
}
