
namespace DiscoverEgypt.Domain.Entities;

public class Refund : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public int? DisputeId { get; set; }
    public Dispute? Dispute { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? DecidedByAdminId { get; set; }
    public DateTime CreatedAt { get; set; }
}

