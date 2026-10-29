namespace DiscoverEgypt.Presentation.ViewModels.Reviews;

public sealed class HostReviewItemViewModel
{
    public int ReviewId { get; init; }
    public int BookingId { get; init; }
    public string ExperienceTitle { get; init; } = string.Empty;
    public string GuestName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public DateTime CreatedAt { get; init; }
}
