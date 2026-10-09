using IterativeProject1.DTOs;
using IterativeProject1.IRepositories;
using IterativeProject1.IServices;
using IterativeProject1.Models;
using IterativeProject1.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IterativeProject1.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IUserRepository _userRepository;

        public NotificationService(
            INotificationPublisher notificationPublisher,
            INotificationRepository notificationRepository,
            IUserRepository userRepository)
        {
            _notificationPublisher = notificationPublisher;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public async Task<int> Create(Notification notification)
        {
            return await _notificationRepository.Create(notification);
        }

        public async Task<IEnumerable<Notification>> GetByUserId(int userId, bool unreadOnly)
        {
            return await _notificationRepository.GetByUserId(userId, unreadOnly);
        }

        public async Task<bool> MarkAsRead(int notificationId, int requestingUserId)
        {
            return await _notificationRepository.MarkAsRead(notificationId, requestingUserId);
        }
        public async Task<bool> MarkAllAsRead(int userId)
        {
            return await _notificationRepository.MarkAllAsRead(userId);
        }

        public async Task<int> GetUnreadCount(int userId)
        {
            return await _notificationRepository.GetUnreadCount(userId);
        }

        public async Task<bool> MarkAsUnread(int notificationId, int requestingUserId)
        {
            return await _notificationRepository.MarkAsUnread(notificationId, requestingUserId);
        }

        public async Task CreateBookingCancelledNotification(Booking booking)
        {
            var recipientUserId = booking.ProviderId;

            var client = await _userRepository.GetById(booking.ClientId);

            string message = $"Booking on {booking.StartTime:MMMM d} at {booking.StartTime:h:mm tt} " +
                              $"cancelled by {client.FullName}";

            var notification = new Notification
            {
                UserId = recipientUserId,
                Type = "Booking Cancellation",
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RelatedBookingId = booking.Id // adjust to your actual Booking PK property name
            };

            var newId = await _notificationRepository.Create(notification);
            notification.NotificationId = newId;

            var notificationDto = new NotificationDto
            {
                Id = notification.NotificationId,
                Type = notification.Type,
                Message = notification.Message,
                RelatedBookingId = notification.RelatedBookingId,
                CreatedAt = notification.CreatedAt
            };

            await _notificationPublisher.PublishToUser(recipientUserId, notificationDto);
        }
    }
}