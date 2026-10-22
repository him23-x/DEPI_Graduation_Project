using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class Payout : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime EligibleAt { get; set; }
    public PayoutStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
}

