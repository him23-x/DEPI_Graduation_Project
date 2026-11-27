namespace DiscoverEgypt.Application.DTOs.Bookings
{
    public sealed class ExperienceBookingInputDto
    {
        public int ExperienceId { get; set; }
        public DateTime? ScheduledStartLocal { get; set; }
        public int? NumberOfGuests { get; set; }
    }
}
