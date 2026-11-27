namespace DiscoverEgypt.Application.DTOs.Experiences
{
    public sealed record ExploreExperienceDto(
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
        int ReviewCount);
}
