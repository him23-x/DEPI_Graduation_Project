using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.Notifications;

public sealed record HostNotificationItem(
    int Id,
    NotificationType Type,
    string Message,
    bool IsRead,
    DateTime CreatedAt,
    int? BookingId);
