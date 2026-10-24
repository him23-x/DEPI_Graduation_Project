using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;

namespace DiscoverEgypt.Infrastructure.Storage;

public sealed class AzureExperienceEvidenceStorage(string? serviceUri, string? connectionString, string? containerName) : IExperienceEvidenceStorage
{
    private BlobContainerClient? container;
    private readonly object sync = new();

    public async Task UploadAsync(string storageKey, string contentType, Stream content, CancellationToken cancellationToken = default)
    {
        var target = GetContainer();
        await target.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        var properties = await target.GetPropertiesAsync(cancellationToken: cancellationToken);
        if (properties.Value.PublicAccess is not null)
            throw new InvalidOperationException("The experience evidence container must have anonymous access disabled.");
        await target.GetBlobClient(storageKey).UploadAsync(content, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        }, cancellationToken);
    }

    public async Task<ExperienceEvidenceDownload> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var response = await GetContainer().GetBlobClient(storageKey).DownloadStreamingAsync(cancellationToken: cancellationToken);
        return new ExperienceEvidenceDownload(response.Value.Content, response.Value.Details.ContentType ?? "application/octet-stream");
    }

    public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
        => await GetContainer().GetBlobClient(storageKey).DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);

    private BlobContainerClient GetContainer()
    {
        if (container is not null) return container;
        lock (sync)
        {
            if (container is not null) return container;
            BlobServiceClient serviceClient;
            if (!string.IsNullOrWhiteSpace(serviceUri))
                serviceClient = new BlobServiceClient(new Uri(serviceUri), new DefaultAzureCredential());
            else if (!string.IsNullOrWhiteSpace(connectionString))
                serviceClient = new BlobServiceClient(connectionString);
            else
                throw new InvalidOperationException("Configure ExperienceEvidenceStorage:ServiceUri or ConnectionString before uploading experience evidence.");
            container = serviceClient.GetBlobContainerClient(string.IsNullOrWhiteSpace(containerName) ? "experience-evidence" : containerName);
            return container;
        }
    }
}
