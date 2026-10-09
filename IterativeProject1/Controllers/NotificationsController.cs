using IterativeProject1.DTOs;
using IterativeProject1.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IterativeProject1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(idClaim);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotification([FromQuery] bool unreadOnly = false)
        {
            var userId = GetCurrentUserId();
            var notifications = await _notificationService.GetByUserId(userId, unreadOnly);

            var ordered = notifications.OrderByDescending(n => n.CreatedAt);
            return Ok(ordered);
        }


        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnReadCount()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUnreadCount(userId);
            return Ok(new { unreadCount = result });
        }


        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _notificationService.MarkAsRead(id, userId);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            await _notificationService.MarkAllAsRead(userId);
            return NoContent();
        }

        [HttpPatch("{id}/Unread")]
        public async Task<IActionResult> MarkAsUnread(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _notificationService.MarkAsUnread(id, userId);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
