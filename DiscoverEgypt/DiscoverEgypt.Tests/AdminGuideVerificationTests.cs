using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.DTOs.GuideVerification;
using DiscoverEgypt.Application.Enums.GuideVerification;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Handlers.Commands;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Handlers.Queries;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Commands;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Queries;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Validators;
using DiscoverEgypt.Application.Responses.GuideVerification;
using DiscoverEgypt.Domain.Enums.Verification;
using DiscoverEgypt.Presentation.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Tests
{
    public sealed class AdminGuideVerificationTests
    {
        [Fact]
        public async Task QueueHandler_ReturnsPendingRequestsFromRepository()
        {
            var item = new AdminGuideVerificationQueueItemResponse(
                3, 9, "Guide One", new DateOnly(2027, 6, 1), DateTime.UtcNow, 4);
            var repository = new FakeRepository { Queue = [item] };
            var handler = new GetAdminGuideVerificationQueueQueryHandler(repository);

            var result = await handler.Handle(new GetAdminGuideVerificationQueueQuery(), CancellationToken.None);

            Assert.Same(item, Assert.Single(result));
        }

        [Fact]
        public async Task DetailsValidator_RejectsInvalidRequestId()
        {
            var validator = new GetAdminGuideVerificationDetailsQueryValidator();

            var result = await validator.ValidateAsync(new GetAdminGuideVerificationDetailsQuery(0));

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ApproveCommand_StoresAdminAndReviewTimeAndClearsReason()
        {
            var reviewedAt = new DateTimeOffset(2026, 11, 24, 15, 30, 0, TimeSpan.Zero);
            var repository = new FakeRepository();
            var handler = CreateDecisionHandler(repository, reviewedAt);

            var result = await handler.Handle(new DecideAdminGuideVerificationCommand(
                "admin-42", 3, GuideVerificationDecision.Approve,
                new AdminGuideVerificationDecisionDto()), CancellationToken.None);

            Assert.Equal(GuideVerificationDecisionOutcome.Success, result.Outcome);
            Assert.Equal("admin-42", repository.ReviewedByAdminId);
            Assert.Equal(reviewedAt.UtcDateTime, repository.ReviewedAtUtc);
            Assert.Null(repository.RejectionReason);
        }

        [Fact]
        public async Task RejectCommand_TrimsAndStoresReason()
        {
            var repository = new FakeRepository();
            var handler = CreateDecisionHandler(repository, DateTimeOffset.UtcNow);

            var result = await handler.Handle(new DecideAdminGuideVerificationCommand(
                "admin-42", 3, GuideVerificationDecision.Reject,
                new AdminGuideVerificationDecisionDto { RejectionReason = "  License details are unclear  " }), CancellationToken.None);

            Assert.Equal(GuideVerificationDecisionOutcome.Success, result.Outcome);
            Assert.Equal("License details are unclear", repository.RejectionReason);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RejectValidator_RequiresReason(string reason)
        {
            var command = new DecideAdminGuideVerificationCommand(
                "admin-42", 3, GuideVerificationDecision.Reject,
                new AdminGuideVerificationDecisionDto { RejectionReason = reason });

            var result = await new DecideAdminGuideVerificationCommandValidator().ValidateAsync(command);

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task RejectValidator_EnforcesConfiguredReasonLimit()
        {
            var command = new DecideAdminGuideVerificationCommand(
                "admin-42", 3, GuideVerificationDecision.Reject,
                new AdminGuideVerificationDecisionDto { RejectionReason = new string('x', 1001) });

            var result = await new DecideAdminGuideVerificationCommandValidator().ValidateAsync(command);

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task DecisionHandler_ReportsRequestThatIsNoLongerPending()
        {
            var repository = new FakeRepository { DecisionOutcome = GuideVerificationDecisionOutcome.NotPending };
            var handler = CreateDecisionHandler(repository, DateTimeOffset.UtcNow);

            var result = await handler.Handle(new DecideAdminGuideVerificationCommand(
                "admin-42", 3, GuideVerificationDecision.Approve,
                new AdminGuideVerificationDecisionDto()), CancellationToken.None);

            Assert.Equal(GuideVerificationDecisionOutcome.NotPending, result.Outcome);
        }

        [Fact]
        public async Task DocumentHandler_DoesNotReadStorageWhenRequestIsNotAvailableForReview()
        {
            var repository = new FakeRepository { Document = null };
            var storage = new FakeEvidenceStorage();
            var handler = new GetAdminGuideVerificationDocumentQueryHandler(repository, storage);

            var result = await handler.Handle(new GetAdminGuideVerificationDocumentQuery(11), CancellationToken.None);

            Assert.Null(result);
            Assert.Null(storage.OpenedStorageKey);
        }

        [Fact]
        public async Task DocumentHandler_OpensAuthorizedPrivateFileWithoutReturningItsKey()
        {
            var repository = new FakeRepository
            {
                Document = new StoredGuideVerificationDocument(11, VerificationDocType.License, "private/license/11")
            };
            var storage = new FakeEvidenceStorage();
            var handler = new GetAdminGuideVerificationDocumentQueryHandler(repository, storage);

            var result = await handler.Handle(new GetAdminGuideVerificationDocumentQuery(11), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("private/license/11", storage.OpenedStorageKey);
            Assert.DoesNotContain("StorageKey", typeof(AdminGuideVerificationDetailsResponse).GetProperties().Select(property => property.Name));
            Assert.DoesNotContain("StorageKey", typeof(AdminGuideVerificationDocumentResponse).GetProperties().Select(property => property.Name));
        }

        [Fact]
        public void AdminVerificationPages_RequireAdminRole()
        {
            var attribute = typeof(GuideVerificationsController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Cast<AuthorizeAttribute>().Single();

            Assert.Equal("Admin", attribute.Roles);
        }

        [Fact]
        public void DecisionActions_RequireAntiforgery()
        {
            foreach (var actionName in new[]
                     {
                         nameof(GuideVerificationsController.Approve),
                         nameof(GuideVerificationsController.Reject)
                     })
            {
                var method = typeof(GuideVerificationsController).GetMethod(actionName);

                Assert.NotNull(method);
                Assert.NotEmpty(method!.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), inherit: true));
            }
        }

        private static DecideAdminGuideVerificationCommandHandler CreateDecisionHandler(
            FakeRepository repository,
            DateTimeOffset reviewedAt) => new(
                repository,
                new DecideAdminGuideVerificationCommandValidator(),
                new FixedTimeProvider(reviewedAt));

        private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
        }

        private sealed class FakeRepository : IAdminGuideVerificationRepository
        {
            public IReadOnlyList<AdminGuideVerificationQueueItemResponse> Queue { get; set; } = [];
            public AdminGuideVerificationDetailsResponse? Details { get; set; }
            public StoredGuideVerificationDocument? Document { get; set; }
            public GuideVerificationDecisionOutcome DecisionOutcome { get; set; } = GuideVerificationDecisionOutcome.Success;
            public string? ReviewedByAdminId { get; private set; }
            public DateTime? ReviewedAtUtc { get; private set; }
            public string? RejectionReason { get; private set; }

            public Task<IReadOnlyList<AdminGuideVerificationQueueItemResponse>> GetPendingAsync(
                CancellationToken cancellationToken = default) => Task.FromResult(Queue);

            public Task<AdminGuideVerificationDetailsResponse?> GetPendingDetailsAsync(
                int verificationRequestId,
                CancellationToken cancellationToken = default) => Task.FromResult(Details);

            public Task<StoredGuideVerificationDocument?> GetDocumentForReviewAsync(
                int documentId,
                CancellationToken cancellationToken = default) => Task.FromResult(Document);

            public Task<GuideVerificationDecisionOutcome> DecideAsync(
                string adminUserId,
                int verificationRequestId,
                GuideVerificationDecision decision,
                string? rejectionReason,
                DateTime reviewedAtUtc,
                CancellationToken cancellationToken = default)
            {
                ReviewedByAdminId = adminUserId;
                ReviewedAtUtc = reviewedAtUtc;
                RejectionReason = rejectionReason;
                return Task.FromResult(DecisionOutcome);
            }
        }

        private sealed class FakeEvidenceStorage : IExperienceEvidenceStorage
        {
            public string? OpenedStorageKey { get; private set; }

            public Task UploadAsync(string storageKey, string contentType, Stream content,
                CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task<ExperienceEvidenceDownload> OpenReadAsync(string storageKey,
                CancellationToken cancellationToken = default)
            {
                OpenedStorageKey = storageKey;
                return Task.FromResult(new ExperienceEvidenceDownload(new MemoryStream([1, 2, 3]), "application/pdf"));
            }

            public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default) => Task.CompletedTask;
        }
    }
}
