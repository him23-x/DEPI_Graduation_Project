using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class Review : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

