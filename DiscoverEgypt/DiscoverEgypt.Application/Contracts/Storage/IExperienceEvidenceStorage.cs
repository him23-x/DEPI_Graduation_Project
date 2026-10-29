using DiscoverEgypt.Application.DTOs.HostExperiences;

using DiscoverEgypt.Application.Contracts.Storage;

namespace DiscoverEgypt.Application.Contracts.Storage;

public interface IExperienceEvidenceStorage
{
    Task UploadAsync(string storageKey, string contentType, Stream content, CancellationToken cancellationToken = default);
    Task<ExperienceEvidenceDownload> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}
