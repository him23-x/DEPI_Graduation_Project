namespace DiscoverEgypt.Application.DTOs.Reviews
{
    public sealed class CreateTouristReviewDto
    {
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
