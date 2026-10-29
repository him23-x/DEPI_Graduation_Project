namespace DiscoverEgypt.Application.DTOs.Reviews;

public sealed record HostReviewListItemDto(
    int ReviewId,
    int BookingId,
    string ExperienceTitle,
    string GuestName,
    int Rating,
    string? Comment,
    DateTime CreatedAt);
