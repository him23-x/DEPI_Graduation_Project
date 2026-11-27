using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Application.Enums.Disputes;
using DiscoverEgypt.Application.Features.AdminDisputes.Handlers.Commands;
using DiscoverEgypt.Application.Features.AdminDisputes.Handlers.Queries;
using DiscoverEgypt.Application.Features.AdminDisputes.Requests.Commands;
using DiscoverEgypt.Application.Features.AdminDisputes.Requests.Queries;
using DiscoverEgypt.Application.Features.AdminDisputes.Validators;
using DiscoverEgypt.Application.Responses.Disputes;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Presentation.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Tests
{
    public sealed class AdminDisputeReviewTests
    {
        [Fact]
        public async Task QueueHandler_ReturnsOpenDisputesFromRepository()
        {
            var item = new AdminDisputeQueueItemResponse(
                12, 47, "Cairo Food Walk", BookingType.Experience, "Tourist One", "Provider One",
                BookingStatus.Completed, DisputeStatus.Open, DateTime.UtcNow);
            var repository = new FakeRepository { Queue = [item] };
            var handler = new GetAdminDisputeQueueQueryHandler(repository);

            var result = await handler.Handle(new GetAdminDisputeQueueQuery(), CancellationToken.None);

            Assert.Same(item, Assert.Single(result));
        }

        [Fact]
        public async Task DetailsValidator_RejectsInvalidDisputeId()
        {
            var result = await new GetAdminDisputeDetailsQueryValidator()
                .ValidateAsync(new GetAdminDisputeDetailsQuery(0));

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ProviderFault_UsesFourteenDayDefaultAndUtcResolutionTime()
        {
            var now = new DateTimeOffset(2026, 11, 24, 15, 30, 0, TimeSpan.Zero);
            var repository = new FakeRepository();
            var handler = CreateHandler(repository, now);

            var result = await handler.Handle(CreateCommand(DisputeOutcome.ProviderFault), CancellationToken.None);

            Assert.Equal(AdminDisputeResolutionOutcome.Success, result.Outcome);
            Assert.Equal("admin-42", repository.AdminUserId);
            Assert.Equal(DisputeOutcome.ProviderFault, repository.Outcome);
            Assert.Equal(14, repository.SanctionDays);
            Assert.Equal(now.UtcDateTime, repository.ResolvedAtUtc);
        }

        [Fact]
        public async Task TouristFault_UsesThirtyDayDefault()
        {
            var repository = new FakeRepository();
            var handler = CreateHandler(repository, DateTimeOffset.UtcNow);

            await handler.Handle(CreateCommand(DisputeOutcome.TouristFault), CancellationToken.None);

            Assert.Equal(DisputeOutcome.TouristFault, repository.Outcome);
            Assert.Equal(30, repository.SanctionDays);
        }

        [Fact]
        public async Task AdminCanChooseSanctionDurationForEachFaultOutcome()
        {
            var providerRepository = new FakeRepository();
            await CreateHandler(providerRepository, DateTimeOffset.UtcNow).Handle(
                CreateCommand(DisputeOutcome.ProviderFault, sanctionDays: 9), CancellationToken.None);
            var touristRepository = new FakeRepository();
            await CreateHandler(touristRepository, DateTimeOffset.UtcNow).Handle(
                CreateCommand(DisputeOutcome.TouristFault, sanctionDays: 21), CancellationToken.None);

            Assert.Equal(9, providerRepository.SanctionDays);
            Assert.Equal(21, touristRepository.SanctionDays);
        }

        [Fact]
        public async Task NoFault_CreatesNoSanction()
        {
            var repository = new FakeRepository();

            await CreateHandler(repository, DateTimeOffset.UtcNow).Handle(
                CreateCommand(DisputeOutcome.NoFault), CancellationToken.None);

            Assert.Equal(DisputeOutcome.NoFault, repository.Outcome);
            Assert.Null(repository.SanctionDays);
        }

        [Fact]
        public async Task ResolutionHandler_ReportsAlreadyResolvedDispute()
        {
            var repository = new FakeRepository
            {
                ResolutionOutcome = AdminDisputeResolutionOutcome.DisputeAlreadyResolved
            };

            var result = await CreateHandler(repository, DateTimeOffset.UtcNow).Handle(
                CreateCommand(DisputeOutcome.NoFault), CancellationToken.None);

            Assert.Equal(AdminDisputeResolutionOutcome.DisputeAlreadyResolved, result.Outcome);
        }

        [Fact]
        public async Task Validator_RequiresOutcomeAndRejectsInvalidSanctionDurationOrLongNote()
        {
            var validator = new ResolveAdminDisputeCommandValidator();
            var noOutcome = await validator.ValidateAsync(new ResolveAdminDisputeCommand(
                "admin-42", 12, new AdminDisputeResolutionDto()));
            var invalidDays = await validator.ValidateAsync(new ResolveAdminDisputeCommand(
                "admin-42", 12, new AdminDisputeResolutionDto
                {
                    Outcome = DisputeOutcome.ProviderFault,
                    SanctionDays = 0
                }));
            var longNote = await validator.ValidateAsync(new ResolveAdminDisputeCommand(
                "admin-42", 12, new AdminDisputeResolutionDto
                {
                    Outcome = DisputeOutcome.NoFault,
                    ResolutionNote = new string('x', 3001)
                }));

            Assert.False(noOutcome.IsValid);
            Assert.False(invalidDays.IsValid);
            Assert.False(longNote.IsValid);
        }

        [Fact]
        public void AdminDisputePages_RequireAdminRole()
        {
            var attribute = typeof(DisputesController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Cast<AuthorizeAttribute>().Single();

            Assert.Equal("Admin", attribute.Roles);
        }

        [Fact]
        public void ResolveAction_RequiresAntiforgery()
        {
            var method = typeof(DisputesController).GetMethod(nameof(DisputesController.Resolve));

            Assert.NotNull(method);
            Assert.NotEmpty(method!.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), inherit: true));
        }

        private static ResolveAdminDisputeCommand CreateCommand(
            DisputeOutcome outcome,
            int disputeId = 12,
            int? sanctionDays = null) => new(
                "admin-42",
                disputeId,
                new AdminDisputeResolutionDto
                {
                    Outcome = outcome,
                    ResolutionNote = "Decision recorded",
                    SanctionDays = sanctionDays
                });

        private static ResolveAdminDisputeCommandHandler CreateHandler(
            FakeRepository repository,
            DateTimeOffset now) => new(repository, new ResolveAdminDisputeCommandValidator(), new FixedTimeProvider(now));

        private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
        }

        private sealed class FakeRepository : IAdminDisputeReviewRepository
        {
            public IReadOnlyList<AdminDisputeQueueItemResponse> Queue { get; set; } = [];
            public AdminDisputeDetailsResponse? Details { get; set; }
            public AdminDisputeResolutionOutcome ResolutionOutcome { get; set; } = AdminDisputeResolutionOutcome.Success;
            public string? AdminUserId { get; private set; }
            public int DisputeId { get; private set; }
            public DisputeOutcome? Outcome { get; private set; }
            public string? ResolutionNote { get; private set; }
            public int? SanctionDays { get; private set; }
            public DateTime? ResolvedAtUtc { get; private set; }

            public Task<IReadOnlyList<AdminDisputeQueueItemResponse>> GetOpenAsync(
                CancellationToken cancellationToken = default) => Task.FromResult(Queue);

            public Task<AdminDisputeDetailsResponse?> GetOpenDetailsAsync(
                int disputeId,
                CancellationToken cancellationToken = default) => Task.FromResult(Details);

            public Task<AdminDisputeResolutionOutcome> ResolveAsync(
                string adminUserId,
                int disputeId,
                DisputeOutcome outcome,
                string? resolutionNote,
                int? sanctionDays,
                DateTime resolvedAtUtc,
                CancellationToken cancellationToken = default)
            {
                AdminUserId = adminUserId;
                DisputeId = disputeId;
                Outcome = outcome;
                ResolutionNote = resolutionNote;
                SanctionDays = sanctionDays;
                ResolvedAtUtc = resolvedAtUtc;
                return Task.FromResult(ResolutionOutcome);
            }
        }
    }
}
