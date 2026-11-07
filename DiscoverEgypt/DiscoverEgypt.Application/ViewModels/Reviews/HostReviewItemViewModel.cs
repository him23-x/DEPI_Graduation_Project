namespace DiscoverEgypt.Application.ViewModels.Reviews
{
    public sealed record HostReviewItemViewModel(
        int ReviewId,
        int BookingId,
        string ExperienceTitle,
        string GuestName,
        int Rating,
        string? Comment,
        DateTime CreatedAt);
}

