using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostExperienceMediaRepository
{
    Task<HostExperienceMediaEditor?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default);
    Task<StoredExperienceMedia?> GetOwnedMediaAsync(string userId, int mediaId, CancellationToken cancellationToken = default);
    Task<ExperienceMediaMutationResult> AddAsync(string userId, int experienceId, MediaType mediaType, string storageKey, CancellationToken cancellationToken = default);
    Task<(ExperienceMediaMutationResult Result, string? StorageKey)> DeleteAsync(string userId, int experienceId, int mediaId, CancellationToken cancellationToken = default);
    Task<ExperienceMediaMutationResult> SetCoverAsync(string userId, int experienceId, int mediaId, CancellationToken cancellationToken = default);
    Task<ExperienceMediaMutationResult> MoveAsync(string userId, int experienceId, int mediaId, int direction, CancellationToken cancellationToken = default);
}
