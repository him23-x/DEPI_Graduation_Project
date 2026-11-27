using System.Security.Claims;
using DiscoverEgypt.Application.Contracts.Notifications;
using DiscoverEgypt.Application.Enums.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Notifications
{
    [Authorize(Roles = "Tourist")]
    [Route("Tourist/Notifications")]
    public sealed class TouristNotificationsController : Controller
    {
        private const int PageSize = 20;
        private readonly INotificationService _notificationService;

        public TouristNotificationsController(INotificationService notificationService) =>
            _notificationService = notificationService;

        [HttpGet("")]
        public async Task<IActionResult> Index(
            NotificationReadFilter filter = NotificationReadFilter.All,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            var userId = CurrentUserId();
            if (userId is null) return Challenge();
            if (!Enum.IsDefined(filter)) filter = NotificationReadFilter.All;

            var notifications = await _notificationService.GetForUserAsync(
                userId, filter, page, PageSize, cancellationToken);
            ViewData["Title"] = "My notifications";
            ViewData["TouristPage"] = "notifications";
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
            var userId = CurrentUserId();
            if (userId is null) return Challenge();
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
            var userId = CurrentUserId();
            if (userId is null) return Challenge();
            var changedCount = await _notificationService.MarkAllReadAsync(userId, cancellationToken);
            if (changedCount > 0) TempData["TouristNotificationsNotice"] = "All notifications are marked as read.";

            return RedirectToAction(nameof(Index), new { filter, page = 1 });
        }

        private string? CurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
