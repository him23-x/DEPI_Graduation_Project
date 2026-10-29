using DiscoverEgypt.Application.Enums.Notifications;

namespace DiscoverEgypt.Application.DTOs.Notifications;

public sealed record HostNotificationsPageDto(
    IReadOnlyList<HostNotificationItemDto> Notifications,
    NotificationReadFilter Filter,
    int Page,
    int PageSize,
    int TotalCount,
    int UnreadCount)
{
    public int PageCount => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}
