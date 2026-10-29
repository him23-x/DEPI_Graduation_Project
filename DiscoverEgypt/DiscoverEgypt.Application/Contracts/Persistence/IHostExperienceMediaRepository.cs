using DiscoverEgypt.Application.DTOs.HostExperiences;

using DiscoverEgypt.Application.Contracts.Storage;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostExperienceMediaRepository
{
    Task<HostExperienceMediaEditorDto?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default);
    Task<StoredExperienceMedia?> GetOwnedMediaAsync(string userId, int mediaId, CancellationToken cancellationToken = default);
    Task<ExperienceMediaMutationOutcome> AddAsync(string userId, int experienceId, MediaType mediaType, string storageKey, CancellationToken cancellationToken = default);
    Task<(ExperienceMediaMutationOutcome Result, string? StorageKey)> DeleteAsync(string userId, int experienceId, int mediaId, CancellationToken cancellationToken = default);
    Task<ExperienceMediaMutationOutcome> SetCoverAsync(string userId, int experienceId, int mediaId, CancellationToken cancellationToken = default);
    Task<ExperienceMediaMutationOutcome> MoveAsync(string userId, int experienceId, int mediaId, int direction, CancellationToken cancellationToken = default);
}
