using System.Security.Claims;
using DiscoverEgypt.Application.Contracts.Notifications;
using DiscoverEgypt.Application.Responses.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DiscoverEgypt.Application.Enums.Notifications;

namespace DiscoverEgypt.Presentation.Controllers.Notifications
{
    [Authorize(Roles = "Host")]
    [Route("Host/Notifications")]
    public sealed class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private const int PageSize = 20;

        [HttpGet("")]
        public async Task<IActionResult> Index(
            NotificationReadFilter filter = NotificationReadFilter.All,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (!Enum.IsDefined(filter)) filter = NotificationReadFilter.All;

            var notifications = await _notificationService.GetForUserAsync(
                userId, filter, page, PageSize, cancellationToken);
            ViewData["Title"] = "Notifications";
            ViewData["Section"] = "Notifications";
            return View(notifications);
        }

        [HttpPost("{id:int}/read")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(
            int id,
            NotificationReadFilter filter = NotificationReadFilter.All,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (id <= 0) return NotFound();

            if (!await _notificationService.MarkReadAsync(id, userId, cancellationToken)) return NotFound();
            return RedirectToAction(nameof(Index), new { filter, page });
        }

        [HttpPost("read-all")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead(
            NotificationReadFilter filter = NotificationReadFilter.All,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            await _notificationService.MarkAllReadAsync(userId, cancellationToken);
            return RedirectToAction(nameof(Index), new { filter, page = 1 });
        }
    }
}

