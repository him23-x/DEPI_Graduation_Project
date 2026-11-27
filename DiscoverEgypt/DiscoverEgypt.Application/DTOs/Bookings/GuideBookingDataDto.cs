namespace DiscoverEgypt.Application.DTOs.Bookings
{
    public sealed record GuideBookingDataDto(
        int GuideProfileId,
        string GuideName,
        string Bio,
        decimal HourlyRate,
        string? ProfileImageKey,
        IReadOnlyList<string> Languages,
        IReadOnlyList<string> CulturalAreas,
        decimal AverageRating,
        int ReviewCount,
        string ProviderUserId);
}
