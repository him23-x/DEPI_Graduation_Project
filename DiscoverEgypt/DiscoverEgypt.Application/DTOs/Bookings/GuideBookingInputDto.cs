namespace DiscoverEgypt.Application.DTOs.Bookings
{
    public sealed class GuideBookingInputDto
    {
        public int GuideProfileId { get; set; }
        public DateTime? ScheduledStartLocal { get; set; }
        public int? GuideHours { get; set; }
        public string? Destination { get; set; }
    }
}
