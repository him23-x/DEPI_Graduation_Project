using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceMediaItem(
    int Id,
    MediaType MediaType,
    string ContentType,
    int DisplayOrder,
    bool IsCover);

public sealed record HostExperienceMediaEditor(
    int ExperienceId,
    string Title,
    IReadOnlyList<HostExperienceMediaItem> Media);

public sealed record StoredExperienceMedia(string StorageKey, MediaType MediaType);

public sealed record ExperienceMediaDownload(Stream Content, string ContentType);

public enum ExperienceMediaMutationResult
{
    Success,
    ExperienceUnavailable,
    ImageLimitReached,
    VideoLimitReached,
    InvalidFile,
    MediaUnavailable,
    CoverMustBeImage,
    InvalidOrder
}
