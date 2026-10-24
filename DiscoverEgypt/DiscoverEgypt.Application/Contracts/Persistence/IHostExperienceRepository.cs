using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostExperienceRepository
{
    Task<IReadOnlyList<HostExperienceListItem>> GetForHostAsync(
        string userId,
        ExperienceStatus? status,
        CancellationToken cancellationToken = default);

    Task<HostExperienceCreateOptions> GetCreateOptionsAsync(CancellationToken cancellationToken = default);

    Task<HostExperienceDraftEditor?> GetEditableDraftAsync(string userId, int experienceId, CancellationToken cancellationToken = default);

    Task<int?> CreateDraftAsync(
        string userId,
        CreateHostExperienceDraftRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateDraftAsync(string userId, int experienceId, CreateHostExperienceDraftRequest request, CancellationToken cancellationToken = default);
}
