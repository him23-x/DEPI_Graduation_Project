using DiscoverEgypt.Application.DTOs.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostExperienceRepository
{
    Task<IReadOnlyList<HostExperienceListItemDto>> GetForHostAsync(
        string userId,
        ExperienceStatus? status,
        CancellationToken cancellationToken = default);

    Task<HostExperienceCreateOptionsDto> GetCreateOptionsAsync(CancellationToken cancellationToken = default);

    Task<HostExperienceDraftEditorDto?> GetEditableDraftAsync(string userId, int experienceId, CancellationToken cancellationToken = default);

    Task<int?> CreateDraftAsync(
        string userId,
        CreateHostExperienceDraftDto request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateDraftAsync(string userId, int experienceId, CreateHostExperienceDraftDto request, CancellationToken cancellationToken = default);
}
