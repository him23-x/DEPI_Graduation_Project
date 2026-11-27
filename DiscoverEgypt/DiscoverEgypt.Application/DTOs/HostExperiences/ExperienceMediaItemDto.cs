
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences
{
    public sealed record ExperienceMediaItemDto(
        int Id,
        MediaType MediaType,
        string ContentType,
        int DisplayOrder,
        bool IsCover);
}

