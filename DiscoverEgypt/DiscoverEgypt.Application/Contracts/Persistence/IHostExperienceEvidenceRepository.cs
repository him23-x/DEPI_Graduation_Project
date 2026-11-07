using DiscoverEgypt.Application.Contracts.Storage;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IHostExperienceEvidenceRepository
    {
        Task<DiscoverEgypt.Domain.Entities.Experience?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default);
        Task<ExperienceEvidenceMutationOutcome> ExecuteMutationAsync(
            string userId,
            int experienceId,
            Func<HostExperienceEvidenceMutationContext, CancellationToken, Task<ExperienceEvidenceMutationOutcome>> action,
            CancellationToken cancellationToken = default);
        Task<StoredExperienceEvidence?> GetOwnedFileAsync(string userId, int evidenceId, CancellationToken cancellationToken = default);
        Task<StoredExperienceEvidence?> GetFileForReviewAsync(int evidenceId, CancellationToken cancellationToken = default);
    }
}

