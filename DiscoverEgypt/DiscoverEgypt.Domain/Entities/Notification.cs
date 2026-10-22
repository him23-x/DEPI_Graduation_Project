using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class Notification : BaseEntity
{
    public string UserId { get; set; }
    public int? BookingId { get; set; }
    public Booking? Booking { get; set; }
    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

