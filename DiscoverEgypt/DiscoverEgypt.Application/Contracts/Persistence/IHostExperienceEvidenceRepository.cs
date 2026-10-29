using DiscoverEgypt.Application.DTOs.HostExperiences;

using DiscoverEgypt.Application.Contracts.Storage;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostExperienceEvidenceRepository
{
    Task<HostExperienceEvidenceEditorDto?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default);
    Task<ExperienceEvidenceMutationOutcome> SaveTextAsync(string userId, int experienceId, EvidenceKind kind, string text, CancellationToken cancellationToken = default);
    Task<ExperienceEvidenceMutationOutcome> AddFileAsync(string userId, int experienceId, EvidenceKind kind, string storageKey, CancellationToken cancellationToken = default);
    Task<(ExperienceEvidenceMutationOutcome Result, string? StorageKey)> DeleteFileAsync(string userId, int experienceId, int evidenceId, CancellationToken cancellationToken = default);
    Task<StoredExperienceEvidence?> GetOwnedFileAsync(string userId, int evidenceId, CancellationToken cancellationToken = default);
    Task<StoredExperienceEvidence?> GetFileForReviewAsync(int evidenceId, CancellationToken cancellationToken = default);
}
