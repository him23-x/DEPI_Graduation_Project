using System.Reflection;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using DiscoverEgypt.Domain.Enums.Experiences;
using DiscoverEgypt.Presentation.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;

namespace DiscoverEgypt.Tests;

public sealed class AdminEvidenceDownloadTests
{
    [Fact]
    public async Task Handler_OpensEvidenceReturnedForReview()
    {
        var repository = new FakeEvidenceRepository
        {
            ReviewFile = new StoredExperienceEvidence("experiences/7/evidence/sample.pdf")
        };
        var storage = new FakeEvidenceStorage();
        var handler = new GetHostExperienceEvidenceFileForReviewQueryHandler(repository, storage);

        var result = await handler.Handle(new GetHostExperienceEvidenceFileForReviewQuery(19), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("experiences/7/evidence/sample.pdf", storage.OpenedKey);
        Assert.Equal("application/pdf", result.ContentType);
    }

    [Fact]
    public async Task Handler_DoesNotOpenStorageWhenEvidenceIsMissingOrDeleted()
    {
        var storage = new FakeEvidenceStorage();
        var handler = new GetHostExperienceEvidenceFileForReviewQueryHandler(new FakeEvidenceRepository(), storage);

        var result = await handler.Handle(new GetHostExperienceEvidenceFileForReviewQuery(19), CancellationToken.None);

        Assert.Null(result);
        Assert.Null(storage.OpenedKey);
    }

    [Fact]
    public void AdminDownloadController_RequiresAdminRole()
    {
        var authorization = typeof(ExperienceEvidenceController).GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorization);
        Assert.Equal("Admin", authorization.Roles);
    }

    private sealed class FakeEvidenceRepository : IHostExperienceEvidenceRepository
    {
        public StoredExperienceEvidence? ReviewFile { get; init; }

        public Task<StoredExperienceEvidence?> GetFileForReviewAsync(int evidenceId, CancellationToken cancellationToken = default)
            => Task.FromResult(ReviewFile);

        public Task<StoredExperienceEvidence?> GetOwnedFileAsync(string userId, int evidenceId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<HostExperienceEvidenceEditorDto?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<ExperienceEvidenceMutationOutcome> SaveTextAsync(string userId, int experienceId, EvidenceKind kind, string text, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<ExperienceEvidenceMutationOutcome> AddFileAsync(string userId, int experienceId, EvidenceKind kind, string storageKey, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<(ExperienceEvidenceMutationOutcome Result, string? StorageKey)> DeleteFileAsync(string userId, int experienceId, int evidenceId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }

    private sealed class FakeEvidenceStorage : IExperienceEvidenceStorage
    {
        public string? OpenedKey { get; private set; }

        public Task<ExperienceEvidenceDownload> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
        {
            OpenedKey = storageKey;
            return Task.FromResult(new ExperienceEvidenceDownload(new MemoryStream([1]), "application/pdf"));
        }

        public Task UploadAsync(string storageKey, string contentType, Stream content, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }
}
