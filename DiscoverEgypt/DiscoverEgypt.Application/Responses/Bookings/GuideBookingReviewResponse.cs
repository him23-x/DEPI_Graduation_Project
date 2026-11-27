namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record GuideBookingReviewResponse(
        int GuideProfileId,
        string GuideName,
        string? ProfileImageKey,
        IReadOnlyList<string> Languages,
        IReadOnlyList<string> CulturalAreas,
        DateTime ScheduledStartLocal,
        DateTime ScheduledEndLocal,
        int GuideHours,
        string Destination,
        decimal HourlyRate,
        decimal ServiceAmount,
        decimal PlatformFeeRate,
        decimal PlatformFeeAmount,
        decimal TotalAmount);
}
