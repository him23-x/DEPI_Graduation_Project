namespace DiscoverEgypt.Application.DTOs.Recommendations
{
    public sealed record RecommendedExperienceDto(
        int Id,
        string Title,
        string ExperienceType,
        string CulturalArea,
        string HostName,
        string? CoverImageKey,
        string CulturalStory,
        decimal PricePerPerson,
        int DurationMinutes,
        decimal AverageRating,
        int ReviewCount,
        decimal MatchPercentage,
        IReadOnlyList<string> MatchDetails);
}
