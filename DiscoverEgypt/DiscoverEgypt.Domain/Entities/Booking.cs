
using DiscoverEgypt.Domain.Enums.Bookings;

namespace DiscoverEgypt.Domain.Entities;

public class Booking : BaseEntity
{
    public string TouristUserId { get; set; }
    public string ProviderUserId { get; set; }
    public BookingType BookingType { get; set; }
    public int? ExperienceId { get; set; }
    public Experience? Experience { get; set; }
    public int? GuideProfileId { get; set; }
    public GuideProfile? GuideProfile { get; set; }
    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }
    public int? NumberOfGuests { get; set; }
    public int? GuideHours { get; set; }
    public string? Destination { get; set; }
    public decimal? PricePerPersonSnapshot { get; set; }
    public decimal? HourlyRateSnapshot { get; set; }
    public decimal ServiceAmount { get; set; }
    public decimal PlatformFeeRate { get; set; }
    public decimal PlatformFeeAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Payment Payment { get; set; } = null!;
    public Payout? Payout { get; set; }
    public ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    public Review? Review { get; set; }
    public ICollection<Dispute> Disputes { get; set; } = new List<Dispute>();
    public ICollection<BookingShare> Shares { get; set; } = new List<BookingShare>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

