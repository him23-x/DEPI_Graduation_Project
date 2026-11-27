using DiscoverEgypt.Domain.Enums.Notifications;

namespace DiscoverEgypt.Application.Responses.Notifications
{
    public sealed record NotificationItemResponse(
        int Id,
        NotificationType Type,
        string Message,
        bool IsRead,
        DateTime CreatedAt,
        int? BookingId);
}
