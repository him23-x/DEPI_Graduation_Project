namespace DiscoverEgypt.Application.DTOs.ExperienceDetails
{
    public sealed record ExperienceDetailsDto(
        int Id,
        string Title,
        string ExperienceType,
        string CulturalArea,
        string MeetingPointArea,
        string Description,
        string CulturalStory,
        decimal PricePerPerson,
        int DurationMinutes,
        int MaxGroupSize,
        string HostName,
        string HostBio,
        IReadOnlyList<string> Interests,
        IReadOnlyList<string> Languages,
        IReadOnlyList<ExperienceDetailsPhotoDto> Photos,
        decimal AverageRating,
        int ReviewCount,
        IReadOnlyList<ExperienceDetailsReviewDto> Reviews);
}
