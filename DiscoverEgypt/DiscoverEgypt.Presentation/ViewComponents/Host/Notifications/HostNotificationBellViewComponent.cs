using System.Security.Claims;
using DiscoverEgypt.Application.Contracts.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.ViewComponents.Host.Notifications;

public sealed class HostNotificationBellViewComponent(INotificationService notificationService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return View(0);

        var unreadCount = await notificationService.GetUnreadCountAsync(userId);
        return View(unreadCount);
    }
}
