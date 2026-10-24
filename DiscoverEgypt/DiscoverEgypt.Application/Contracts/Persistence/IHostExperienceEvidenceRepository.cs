using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostExperienceEvidenceRepository
{
    Task<HostExperienceEvidenceEditor?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default);
    Task<ExperienceEvidenceMutationResult> SaveTextAsync(string userId, int experienceId, EvidenceKind kind, string text, CancellationToken cancellationToken = default);
    Task<ExperienceEvidenceMutationResult> AddFileAsync(string userId, int experienceId, EvidenceKind kind, string storageKey, CancellationToken cancellationToken = default);
    Task<(ExperienceEvidenceMutationResult Result, string? StorageKey)> DeleteFileAsync(string userId, int experienceId, int evidenceId, CancellationToken cancellationToken = default);
    Task<StoredExperienceEvidence?> GetOwnedFileAsync(string userId, int evidenceId, CancellationToken cancellationToken = default);
}
