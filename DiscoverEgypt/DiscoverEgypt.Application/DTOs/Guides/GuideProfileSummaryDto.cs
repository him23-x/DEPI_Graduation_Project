namespace DiscoverEgypt.Application.DTOs.Guides
{
    public sealed record GuideProfileSummaryDto(
        int Id,
        string FullName,
        string Bio,
        decimal HourlyRate,
        string? ProfileImageKey,
        IReadOnlyList<string> Languages,
        IReadOnlyList<string> CulturalAreas,
        decimal AverageRating,
        int ReviewCount);
}
