
using DiscoverEgypt.Domain.Enums.Notifications;

namespace DiscoverEgypt.Application.DTOs.Notifications;

public sealed record HostNotificationItemDto(
    int Id,
    NotificationType Type,
    string Message,
    bool IsRead,
    DateTime CreatedAt,
    int? BookingId);
