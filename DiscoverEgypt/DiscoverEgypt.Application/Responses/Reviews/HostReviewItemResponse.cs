namespace DiscoverEgypt.Application.Responses.Reviews
{
    public sealed record HostReviewItemResponse(
        int ReviewId,
        int BookingId,
        string ExperienceTitle,
        string GuestName,
        int Rating,
        string? Comment,
        DateTime CreatedAt);
}
