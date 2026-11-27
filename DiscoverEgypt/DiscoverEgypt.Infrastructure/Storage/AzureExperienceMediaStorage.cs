using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;

namespace DiscoverEgypt.Infrastructure.Storage
{
    public sealed class AzureExperienceMediaStorage : IExperienceMediaStorage
    {
        private readonly string? _serviceUri;
        private readonly string? _connectionString;
        private readonly string? _containerName;

        public AzureExperienceMediaStorage(string? serviceUri, string? connectionString, string? containerName)
        {
            _serviceUri = serviceUri;
            _connectionString = connectionString;
            _containerName = containerName;
        }

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

                if (!string.IsNullOrWhiteSpace(_serviceUri))
                {
                    serviceClient = new BlobServiceClient(new Uri(_serviceUri), new DefaultAzureCredential());
                }
                else if (!string.IsNullOrWhiteSpace(_connectionString))
                {
                    serviceClient = new BlobServiceClient(_connectionString);
                }
                else
                {
                    throw new InvalidOperationException("Configure ExperienceMediaStorage:ServiceUri or ConnectionString before uploading experience media.");
                }

                container = serviceClient.GetBlobContainerClient(string.IsNullOrWhiteSpace(_containerName) ? "experience-media" : _containerName);
                return container;
            }
        }
    }
}

