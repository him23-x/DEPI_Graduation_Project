using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;

namespace DiscoverEgypt.Infrastructure.Storage;

public sealed class AzureExperienceMediaStorage(string? serviceUri, string? connectionString, string? containerName) : IExperienceMediaStorage
{
    private BlobContainerClient? container;
    private readonly object sync = new();

    public async Task UploadAsync(string storageKey, string contentType, Stream content, CancellationToken cancellationToken = default)
    {
        var targetContainer = GetContainer();
        await targetContainer.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
        var containerProperties = await targetContainer.GetPropertiesAsync(cancellationToken: cancellationToken);
        if (containerProperties.Value.PublicAccess is not null)
        {
            throw new InvalidOperationException("The experience media container must have anonymous access disabled.");
        }
        var blob = targetContainer.GetBlobClient(storageKey);
        await blob.UploadAsync(content, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        }, cancellationToken);
    }

    public async Task<ExperienceMediaDownload> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var response = await GetContainer().GetBlobClient(storageKey).DownloadStreamingAsync(cancellationToken: cancellationToken);
        return new ExperienceMediaDownload(response.Value.Content, response.Value.Details.ContentType ?? "application/octet-stream");
    }

    public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        await GetContainer().GetBlobClient(storageKey).DeleteIfExistsAsync(
            DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: cancellationToken);
    }

    private BlobContainerClient GetContainer()
    {
        if (container is not null)
        {
            return container;
        }

        lock (sync)
        {
            if (container is not null)
            {
                return container;
            }

            BlobServiceClient serviceClient;

            if (!string.IsNullOrWhiteSpace(serviceUri))
            {
                serviceClient = new BlobServiceClient(new Uri(serviceUri), new DefaultAzureCredential());
            }
            else if (!string.IsNullOrWhiteSpace(connectionString))
            {
                serviceClient = new BlobServiceClient(connectionString);
            }
            else
            {
                throw new InvalidOperationException("Configure ExperienceMediaStorage:ServiceUri or ConnectionString before uploading experience media.");
            }

            container = serviceClient.GetBlobContainerClient(string.IsNullOrWhiteSpace(containerName) ? "experience-media" : containerName);
            return container;
        }
    }
}
