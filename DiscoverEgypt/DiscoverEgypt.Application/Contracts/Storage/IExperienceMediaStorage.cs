using DiscoverEgypt.Application.DTOs.HostExperiences;

using DiscoverEgypt.Application.Contracts.Storage;

namespace DiscoverEgypt.Application.Contracts.Storage;

public interface IExperienceMediaStorage
{
    Task UploadAsync(string storageKey, string contentType, Stream content, CancellationToken cancellationToken = default);
    Task<ExperienceMediaDownload> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}
