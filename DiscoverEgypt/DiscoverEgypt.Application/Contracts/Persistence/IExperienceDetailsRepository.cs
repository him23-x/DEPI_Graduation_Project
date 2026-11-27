using DiscoverEgypt.Application.DTOs.ExperienceDetails;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IExperienceDetailsRepository
    {
        Task<ExperienceDetailsDto?> GetPublishedDetailsAsync(int experienceId, CancellationToken cancellationToken = default);
        Task<string?> GetPublishedImageKeyAsync(int experienceId, int mediaId, CancellationToken cancellationToken = default);
    }
}
