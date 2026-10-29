using DiscoverEgypt.Application.DTOs.Notifications;

using DiscoverEgypt.Application.Enums.Notifications;

namespace DiscoverEgypt.Presentation.ViewModels.Notifications;

public sealed class HostNotificationsViewModel
{
    public IReadOnlyList<HostNotificationViewModel> Notifications { get; init; } = [];
    public NotificationReadFilter Filter { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int UnreadCount { get; init; }
    public int PageCount => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}
