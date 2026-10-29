using DiscoverEgypt.Application.DTOs.Notifications;

using DiscoverEgypt.Application.Enums.Notifications;

using DiscoverEgypt.Domain.Enums.Notifications;

namespace DiscoverEgypt.Application.Contracts.Notifications;

public interface INotificationService
{
    Task CreateAsync(
        string userId,
        NotificationType type,
        string message,
        int? bookingId = null,
        CancellationToken cancellationToken = default);

    Task<HostNotificationsPageDto> GetForUserAsync(
        string userId,
        NotificationReadFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);

    Task<bool> MarkReadAsync(int notificationId, string userId, CancellationToken cancellationToken = default);

    Task<int> MarkAllReadAsync(string userId, CancellationToken cancellationToken = default);
}
