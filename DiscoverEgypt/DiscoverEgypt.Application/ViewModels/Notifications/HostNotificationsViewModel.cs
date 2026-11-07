using DiscoverEgypt.Application.Enums.Notifications;

namespace DiscoverEgypt.Application.ViewModels.Notifications
{
    public sealed record HostNotificationsViewModel(
        IReadOnlyList<HostNotificationViewModel> Notifications,
        NotificationReadFilter Filter,
        int Page,
        int PageSize,
        int TotalCount,
        int UnreadCount)
    {
        public int PageCount => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    }
}

