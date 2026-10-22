using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class Payment : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

