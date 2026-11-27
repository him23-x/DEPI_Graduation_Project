using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.DTOs.AdminExperiences;
using DiscoverEgypt.Application.Enums.AdminExperiences;
using DiscoverEgypt.Application.Features.AdminExperiences.Handlers.Commands;
using DiscoverEgypt.Application.Features.AdminExperiences.Handlers.Queries;
using DiscoverEgypt.Application.Features.AdminExperiences.Requests.Commands;
using DiscoverEgypt.Application.Features.AdminExperiences.Requests.Queries;
using DiscoverEgypt.Application.Features.AdminExperiences.Validators;
using DiscoverEgypt.Application.Features.HostExperiences.Services;
using DiscoverEgypt.Application.Features.HostExperiences.Validators.DTOs;
using DiscoverEgypt.Application.Responses.AdminExperiences;
using DiscoverEgypt.Domain.Enums.Experiences;
using FluentValidation;
using DiscoverEgypt.Presentation.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Tests
{
    public sealed class AdminExperienceReviewTests
    {
        [Fact]
        public async Task QueueHandler_ReturnsPendingQueueItemsFromRepository()
        {
            var expected = new AdminExperienceQueueItemResponse(
                17, "Old Cairo Food Walk", "Food & Cooking", "Cairo", "Host One",
                new DateTime(2026, 11, 23, 10, 0, 0, DateTimeKind.Utc), 4, 0, ExperienceStatus.PendingApproval);
            var repository = new FakeAdminExperienceReviewRepository([expected]);
            var handler = new GetAdminExperienceQueueQueryHandler(repository);

            var result = await handler.Handle(new GetAdminExperienceQueueQuery(), CancellationToken.None);

            Assert.Same(expected, Assert.Single(result));
        }

        [Fact]
        public async Task DetailsHandler_ReturnsNullWhenExperienceIsNoLongerPending()
        {
            var repository = new FakeAdminExperienceReviewRepository([]);
            var handler = new GetAdminExperienceDetailsQueryHandler(
                repository, new GetAdminExperienceDetailsQueryValidator());

            var result = await handler.Handle(new GetAdminExperienceDetailsQuery(17), CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task DetailsValidator_RejectsInvalidExperienceId()
        {
            var validator = new GetAdminExperienceDetailsQueryValidator();

            var result = await validator.ValidateAsync(new GetAdminExperienceDetailsQuery(0));

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ApproveCommand_SavesAdminAndUtcTimestampAndClearsReason()
        {
            var now = new DateTimeOffset(2026, 11, 24, 14, 30, 0, TimeSpan.Zero);
            var repository = new FakeAdminExperienceReviewRepository([]);
            var handler = CreateDecisionHandler(repository, now);

            var result = await handler.Handle(new DecideAdminExperienceReviewCommand(
                "admin-42", 17, ExperienceReviewDecision.Approve, new AdminExperienceDecisionDto()), CancellationToken.None);

            Assert.Equal(ExperienceReviewDecisionOutcome.Success, result.Outcome);
            Assert.Equal("admin-42", repository.DecidedBy);
            Assert.Equal(17, repository.DecidedExperienceId);
            Assert.Equal(ExperienceReviewDecision.Approve, repository.Decision);
            Assert.Null(repository.RejectionReason);
            Assert.Equal(now.UtcDateTime, repository.DecisionAtUtc);
        }

        [Fact]
        public async Task RejectCommand_TrimsAndSavesReason()
        {
            var repository = new FakeAdminExperienceReviewRepository([]);
            var handler = CreateDecisionHandler(repository, DateTimeOffset.UtcNow);

            var result = await handler.Handle(new DecideAdminExperienceReviewCommand(
                "admin-42", 17, ExperienceReviewDecision.Reject,
                new AdminExperienceDecisionDto { RejectionReason = "  Missing safety evidence  " }), CancellationToken.None);

            Assert.Equal(ExperienceReviewDecisionOutcome.Success, result.Outcome);
            Assert.Equal("Missing safety evidence", repository.RejectionReason);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RejectValidator_RequiresNonEmptyReason(string reason)
        {
            var validator = new DecideAdminExperienceReviewCommandValidator();
            var command = new DecideAdminExperienceReviewCommand(
                "admin-42", 17, ExperienceReviewDecision.Reject,
                new AdminExperienceDecisionDto { RejectionReason = reason });

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task RejectValidator_RejectsReasonOverOneThousandCharacters()
        {
            var validator = new DecideAdminExperienceReviewCommandValidator();
            var command = new DecideAdminExperienceReviewCommand(
                "admin-42", 17, ExperienceReviewDecision.Reject,
                new AdminExperienceDecisionDto { RejectionReason = new string('x', 1001) });

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ApproveValidator_AllowsMissingReason()
        {
            var validator = new DecideAdminExperienceReviewCommandValidator();
            var command = new DecideAdminExperienceReviewCommand(
                "admin-42", 17, ExperienceReviewDecision.Approve, new AdminExperienceDecisionDto());

            var result = await validator.ValidateAsync(command);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task DecisionHandler_ReportsWhenExperienceIsNoLongerPending()
        {
            var repository = new FakeAdminExperienceReviewRepository([])
            {
                DecisionOutcome = ExperienceReviewDecisionOutcome.NotPending
            };
            var handler = CreateDecisionHandler(repository, DateTimeOffset.UtcNow);

            var result = await handler.Handle(new DecideAdminExperienceReviewCommand(
                "admin-42", 17, ExperienceReviewDecision.Approve, new AdminExperienceDecisionDto()), CancellationToken.None);

            Assert.Equal(ExperienceReviewDecisionOutcome.NotPending, result.Outcome);
        }

        [Fact]
        public async Task UnpublishCommand_RecordsAdminAndUsesUnpublishDecision()
        {
            var repository = new FakeAdminExperienceReviewRepository([]);
            var handler = CreateDecisionHandler(repository, DateTimeOffset.UtcNow);

            var result = await handler.Handle(new DecideAdminExperienceReviewCommand(
                "admin-42", 17, ExperienceReviewDecision.Unpublish, new AdminExperienceDecisionDto()), CancellationToken.None);

            Assert.Equal(ExperienceReviewDecisionOutcome.Success, result.Outcome);
            Assert.Equal("admin-42", repository.DecidedBy);
            Assert.Equal(ExperienceReviewDecision.Unpublish, repository.Decision);
            Assert.NotNull(repository.DecisionAtUtc);
        }

        [Fact]
        public async Task RepublishCommand_UsesExistingReadinessRulesAndRecordsDecision()
        {
            var repository = new FakeAdminExperienceReviewRepository([])
            {
                ExperienceDetails = CreatePublishableUnpublishedExperience()
            };
            var handler = CreateDecisionHandler(repository, DateTimeOffset.UtcNow);

            var result = await handler.Handle(new DecideAdminExperienceReviewCommand(
                "admin-42", 17, ExperienceReviewDecision.Republish, new AdminExperienceDecisionDto()), CancellationToken.None);

            Assert.Equal(ExperienceReviewDecisionOutcome.Success, result.Outcome);
            Assert.Equal(ExperienceReviewDecision.Republish, repository.Decision);
            Assert.Equal("admin-42", repository.DecidedBy);
        }

        [Fact]
        public async Task RepublishCommand_RefusesWhenPublicationPrerequisitesAreMissing()
        {
            var repository = new FakeAdminExperienceReviewRepository([])
            {
                ExperienceDetails = CreatePublishableUnpublishedExperience() with
                {
                    ImageCount = 0,
                    HasCoverImage = false,
                    CoverImageCount = 0,
                    Evidence = [],
                    Declarations = []
                }
            };
            var handler = CreateDecisionHandler(repository, DateTimeOffset.UtcNow);

            var result = await handler.Handle(new DecideAdminExperienceReviewCommand(
                "admin-42", 17, ExperienceReviewDecision.Republish, new AdminExperienceDecisionDto()), CancellationToken.None);

            Assert.Equal(ExperienceReviewDecisionOutcome.RepublishPrerequisitesNotMet, result.Outcome);
            Assert.NotEmpty(result.Issues);
            Assert.Null(repository.Decision);
        }

        [Fact]
        public async Task RepublishCommand_RefusesAnExperienceThatIsNotUnpublished()
        {
            var repository = new FakeAdminExperienceReviewRepository([])
            {
                ExperienceDetails = CreatePublishableUnpublishedExperience() with { Status = ExperienceStatus.Published }
            };
            var handler = CreateDecisionHandler(repository, DateTimeOffset.UtcNow);

            var result = await handler.Handle(new DecideAdminExperienceReviewCommand(
                "admin-42", 17, ExperienceReviewDecision.Republish, new AdminExperienceDecisionDto()), CancellationToken.None);

            Assert.Equal(ExperienceReviewDecisionOutcome.NotUnpublished, result.Outcome);
            Assert.Null(repository.Decision);
        }

        [Fact]
        public void DecisionActions_RequireAntiforgery()
        {
            foreach (var actionName in new[]
                     {
                         nameof(ExperiencesController.Approve), nameof(ExperiencesController.Reject),
                         nameof(ExperiencesController.Unpublish), nameof(ExperiencesController.Republish)
                     })
            {
                var method = typeof(ExperiencesController).GetMethod(actionName);

                Assert.NotNull(method);
                Assert.NotEmpty(method!.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), inherit: true));
            }
        }

        [Fact]
        public void EvidenceResponse_DoesNotExposeStorageKeyOrFileBytes()
        {
            var propertyNames = typeof(AdminExperienceEvidenceResponse).GetProperties().Select(property => property.Name);

            Assert.DoesNotContain("StorageKey", propertyNames);
            Assert.DoesNotContain("Content", propertyNames);
            Assert.Contains("HasFile", propertyNames);
        }

        [Fact]
        public void AdminExperiencePages_RequireAdminRole()
        {
            var attribute = typeof(ExperiencesController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Cast<AuthorizeAttribute>().Single();

            Assert.Equal("Admin", attribute.Roles);
        }

        [Fact]
        public void AdminEvidenceDownload_RequiresAdminRole()
        {
            var attribute = typeof(ExperienceEvidenceController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Cast<AuthorizeAttribute>().Single();

            Assert.Equal("Admin", attribute.Roles);
        }

        private static DecideAdminExperienceReviewCommandHandler CreateDecisionHandler(
            FakeAdminExperienceReviewRepository repository,
            DateTimeOffset now) => new(
                repository,
                new DecideAdminExperienceReviewCommandValidator(),
                new FixedTimeProvider(now),
                new ExperienceReadinessService(new HostExperienceSubmissionReviewDtoValidator()));

        private static AdminExperienceDetailsResponse CreatePublishableUnpublishedExperience() => new(
            17,
            "Old Cairo Food Walk",
            "Food & Cooking",
            "Cairo",
            "Host One",
            "A walk with local food.",
            "A story about Cairo's food traditions.",
            500,
            120,
            6,
            "Downtown Cairo",
            "Exact private meeting point",
            new DateTime(2026, 11, 23, 10, 0, 0, DateTimeKind.Utc),
            ["Food"],
            ["English"],
            4,
            0,
            true,
            [
                new AdminExperienceEvidenceResponse(1, EvidenceKind.MenuAndAllergenInfo, "Ingredients and allergens", DateTime.UtcNow, false),
                new AdminExperienceEvidenceResponse(2, EvidenceKind.FoodAreaPhoto, null, DateTime.UtcNow, true),
                new AdminExperienceEvidenceResponse(3, EvidenceKind.FoodAreaPhoto, null, DateTime.UtcNow, true),
                new AdminExperienceEvidenceResponse(4, EvidenceKind.FoodAreaPhoto, null, DateTime.UtcNow, true)
            ],
            [new AdminExperienceDeclarationResponse(DeclarationType.FoodSafety,
                "I confirm that the food served during this experience is prepared hygienically, that ingredients and common allergens are accurately disclosed, and that safe food-handling practices are followed.",
                DateTime.UtcNow)],
            ExperienceStatus.Unpublished,
            null,
            1);

        private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
        }

        private sealed class FakeAdminExperienceReviewRepository(
            IReadOnlyList<AdminExperienceQueueItemResponse> queue) : IAdminExperienceReviewRepository
        {
            public string? DecidedBy { get; private set; }
            public int DecidedExperienceId { get; private set; }
            public ExperienceReviewDecision? Decision { get; private set; }
            public string? RejectionReason { get; private set; }
            public DateTime? DecisionAtUtc { get; private set; }
            public ExperienceReviewDecisionOutcome DecisionOutcome { get; set; } = ExperienceReviewDecisionOutcome.Success;
            public AdminExperienceDetailsResponse? ExperienceDetails { get; set; }

            public Task<IReadOnlyList<AdminExperienceQueueItemResponse>> GetForManagementAsync(
                CancellationToken cancellationToken = default) => Task.FromResult(queue);

            public Task<AdminExperienceDetailsResponse?> GetDetailsAsync(
                int experienceId,
                CancellationToken cancellationToken = default) => Task.FromResult(ExperienceDetails);

            public Task<ExperienceReviewDecisionOutcome> DecideAsync(
                string adminUserId,
                int experienceId,
                ExperienceReviewDecision decision,
                string? rejectionReason,
                DateTime nowUtc,
                CancellationToken cancellationToken = default)
            {
                DecidedBy = adminUserId;
                DecidedExperienceId = experienceId;
                Decision = decision;
                RejectionReason = rejectionReason;
                DecisionAtUtc = nowUtc;
                return Task.FromResult(DecisionOutcome);
            }
        }
    }
}
