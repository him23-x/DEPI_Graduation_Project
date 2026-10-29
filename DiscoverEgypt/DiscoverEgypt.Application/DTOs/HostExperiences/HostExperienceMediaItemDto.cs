
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceMediaItemDto(
    int Id,
    MediaType MediaType,
    string ContentType,
    int DisplayOrder,
    bool IsCover);

