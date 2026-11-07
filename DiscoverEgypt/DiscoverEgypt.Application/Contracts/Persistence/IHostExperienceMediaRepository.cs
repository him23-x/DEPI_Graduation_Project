using DiscoverEgypt.Application.Contracts.Storage;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IHostExperienceMediaRepository
    {
        Task<DiscoverEgypt.Domain.Entities.Experience?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default);
        Task<StoredExperienceMedia?> GetOwnedMediaAsync(string userId, int mediaId, CancellationToken cancellationToken = default);
        Task<ExperienceMediaMutationOutcome> ExecuteMutationAsync(
            string userId,
            int experienceId,
            Func<HostExperienceMediaMutationContext, CancellationToken, Task<ExperienceMediaMutationOutcome>> action,
            CancellationToken cancellationToken = default);
    }
}

