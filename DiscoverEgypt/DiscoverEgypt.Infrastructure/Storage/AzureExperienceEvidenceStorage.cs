using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.ViewModels.HostExperiences;

namespace DiscoverEgypt.Infrastructure.Storage
{
    public sealed class AzureExperienceEvidenceStorage : IExperienceEvidenceStorage
    {
        private readonly string? _serviceUri;
        private readonly string? _connectionString;
        private readonly string? _containerName;

        public AzureExperienceEvidenceStorage(string? serviceUri, string? connectionString, string? containerName)
        {
            _serviceUri = serviceUri;
            _connectionString = connectionString;
            _containerName = containerName;
        }

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
                if (!string.IsNullOrWhiteSpace(_serviceUri))
                    serviceClient = new BlobServiceClient(new Uri(_serviceUri), new DefaultAzureCredential());
                else if (!string.IsNullOrWhiteSpace(_connectionString))
                    serviceClient = new BlobServiceClient(_connectionString);
                else
                    throw new InvalidOperationException("Configure ExperienceEvidenceStorage:ServiceUri or ConnectionString before uploading experience evidence.");
                container = serviceClient.GetBlobContainerClient(string.IsNullOrWhiteSpace(_containerName) ? "experience-evidence" : _containerName);
                return container;
            }
        }
    }
}

