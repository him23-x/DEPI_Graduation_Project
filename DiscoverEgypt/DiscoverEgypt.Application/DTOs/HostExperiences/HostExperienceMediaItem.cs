using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceMediaItem(
    int Id,
    MediaType MediaType,
    string ContentType,
    int DisplayOrder,
    bool IsCover);

