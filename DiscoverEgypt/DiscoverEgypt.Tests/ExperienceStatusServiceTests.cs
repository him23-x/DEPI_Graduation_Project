using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostExperiences.Services;

namespace DiscoverEgypt.Tests;

public sealed class ExperienceStatusServiceTests
{
    [Fact]
    public async Task ReturnToPendingApprovalAsync_DelegatesOwnerAndExperienceToRepository()
    {
        var repository = new FakeExperienceStatusRepository { Result = true };
        IExperienceStatusService service = new ExperienceStatusService(repository);

        var result = await service.ReturnToPendingApprovalAsync("host-123", 42);

        Assert.True(result);
        Assert.Equal("host-123", repository.UserId);
        Assert.Equal(42, repository.ExperienceId);
    }

    [Fact]
    public async Task ReturnToPendingApprovalAsync_ReturnsFalseWhenExperienceCannotTransition()
    {
        var repository = new FakeExperienceStatusRepository { Result = false };
        IExperienceStatusService service = new ExperienceStatusService(repository);

        var result = await service.ReturnToPendingApprovalAsync("host-123", 42);

        Assert.False(result);
    }

    [Fact]
    public async Task ReturnToPendingApprovalAsync_RejectsMissingOwner()
    {
        var service = new ExperienceStatusService(new FakeExperienceStatusRepository());

        await Assert.ThrowsAsync<ArgumentException>(() => service.ReturnToPendingApprovalAsync(" ", 42));
    }

    private sealed class FakeExperienceStatusRepository : IExperienceStatusRepository
    {
        public bool Result { get; init; }
        public string? UserId { get; private set; }
        public int ExperienceId { get; private set; }

        public Task<bool> ReturnToPendingApprovalAsync(
            string userId,
            int experienceId,
            CancellationToken cancellationToken = default)
        {
            UserId = userId;
            ExperienceId = experienceId;
            return Task.FromResult(Result);
        }
    }
}
