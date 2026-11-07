using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Domain.Enums.Experiences;
using DiscoverEgypt.Application.Features.HostExperiences.Validators.Commands;
using FluentValidation;

namespace DiscoverEgypt.Tests
{
    public sealed class ExperienceStatusServiceTests
    {
        [Fact]
        public async Task Handler_TransitionsPublishedExperienceToPendingApproval()
        {
            var repository = new FakeExperienceStatusRepository { Status = ExperienceStatus.Published };
            var handler = new ReturnPublishedExperienceToReviewCommandHandler(repository, new ReturnPublishedExperienceToReviewCommandValidator());

            var result = await handler.Handle(new ReturnPublishedExperienceToReviewCommand("host-123", 42), default);

            Assert.True(result);
            Assert.Equal("host-123", repository.UserId);
            Assert.Equal(42, repository.ExperienceId);
            Assert.Equal(ExperienceStatus.Published, repository.ExpectedStatus);
            Assert.Equal(ExperienceStatus.PendingApproval, repository.NewStatus);
        }

        [Fact]
        public async Task Handler_DoesNotTransitionNonPublishedExperience()
        {
            var repository = new FakeExperienceStatusRepository { Status = ExperienceStatus.Draft };
            var handler = new ReturnPublishedExperienceToReviewCommandHandler(repository, new ReturnPublishedExperienceToReviewCommandValidator());

            var result = await handler.Handle(new ReturnPublishedExperienceToReviewCommand("host-123", 42), default);

            Assert.False(result);
            Assert.False(repository.WasUpdated);
        }

        [Fact]
        public async Task Handler_RejectsMissingOwner()
        {
            var handler = new ReturnPublishedExperienceToReviewCommandHandler(new FakeExperienceStatusRepository(), new ReturnPublishedExperienceToReviewCommandValidator());

            await Assert.ThrowsAsync<ValidationException>(() =>
                handler.Handle(new ReturnPublishedExperienceToReviewCommand(" ", 42), default));
        }

        private sealed class FakeExperienceStatusRepository : IExperienceStatusRepository
        {
            public ExperienceStatus? Status { get; init; }
            public string? UserId { get; private set; }
            public int ExperienceId { get; private set; }
            public ExperienceStatus ExpectedStatus { get; private set; }
            public ExperienceStatus NewStatus { get; private set; }
            public bool WasUpdated { get; private set; }

            public Task<ExperienceStatus?> GetStatusAsync(string userId, int experienceId, CancellationToken cancellationToken = default)
            {
                UserId = userId;
                ExperienceId = experienceId;
                return Task.FromResult(Status);
            }

            public Task<bool> UpdateStatusAsync(
                string userId,
                int experienceId,
                ExperienceStatus expectedStatus,
                ExperienceStatus newStatus,
                string? rejectionReason,
                DateTime updatedAt,
                CancellationToken cancellationToken = default)
            {
                ExpectedStatus = expectedStatus;
                NewStatus = newStatus;
                WasUpdated = true;
                return Task.FromResult(true);
            }
        }
    }
}
