namespace DiscoverEgypt.Application.DTOs.Recommendations
{
    public sealed record RecommendationExperienceCandidateDto(
        int Id,
        DiscoverEgypt.Domain.Enums.Experiences.ExperienceStatus Status,
        string Title,
        string ExperienceType,
        int ExperienceTypeId,
        string CulturalArea,
        int CulturalAreaId,
        string HostName,
        string? CoverImageKey,
        string CulturalStory,
        decimal PricePerPerson,
        int DurationMinutes,
        decimal AverageRating,
        int ReviewCount,
        IReadOnlyList<int> InterestIds,
        IReadOnlyList<int> LanguageIds);
}
