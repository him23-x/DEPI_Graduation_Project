using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;

namespace DiscoverEgypt.Infrastructure.Storage;

public sealed class LocalExperienceMediaStorage(string rootPath) : IExperienceMediaStorage
{
    private readonly LocalExperienceFileStorage _storage = new(rootPath);

    public Task UploadAsync(string storageKey, string contentType, Stream content, CancellationToken cancellationToken = default)
        => _storage.UploadAsync(storageKey, content, cancellationToken);

    public async Task<ExperienceMediaDownload> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
        => new(await _storage.OpenReadAsync(storageKey, cancellationToken), LocalExperienceFileStorage.GetContentType(storageKey));

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
        => _storage.DeleteAsync(storageKey, cancellationToken);
}
