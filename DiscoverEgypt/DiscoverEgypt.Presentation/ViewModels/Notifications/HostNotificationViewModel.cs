
using DiscoverEgypt.Domain.Enums.Notifications;

namespace DiscoverEgypt.Presentation.ViewModels.Notifications;

public sealed record HostNotificationViewModel(
    int Id,
    NotificationType Type,
    string Message,
    bool IsRead,
    DateTime CreatedAt,
    int? BookingId);
