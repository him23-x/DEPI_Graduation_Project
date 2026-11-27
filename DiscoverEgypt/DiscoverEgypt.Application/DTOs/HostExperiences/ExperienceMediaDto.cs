
namespace DiscoverEgypt.Application.DTOs.HostExperiences
{
    public sealed record ExperienceMediaDto(
        int ExperienceId,
        string Title,
        IReadOnlyList<ExperienceMediaItemDto> Media);
}

