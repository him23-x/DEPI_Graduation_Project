using System.Security.Claims;
using DiscoverEgypt.Application.Contracts.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.ViewComponents.Host.Notifications
{
    public sealed class HostNotificationBellViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public HostNotificationBellViewComponent(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return View(0);

            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
            return View(unreadCount);
        }
    }
}

