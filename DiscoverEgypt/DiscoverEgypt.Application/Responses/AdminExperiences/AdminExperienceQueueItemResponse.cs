using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Responses.AdminExperiences
{
    public sealed record AdminExperienceQueueItemResponse(
        int ExperienceId,
        string Title,
        string ExperienceType,
        string CulturalArea,
        string HostName,
        DateTime SubmittedAt,
        int ImageCount,
        int VideoCount,
        ExperienceStatus Status);
}
