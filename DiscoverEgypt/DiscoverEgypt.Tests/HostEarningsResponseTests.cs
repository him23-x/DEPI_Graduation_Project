using AutoMapper;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Earnings.Handlers.Queries;
using DiscoverEgypt.Application.Features.Earnings.Requests.Queries;
using DiscoverEgypt.Application.Features.Earnings.Validators;
using DiscoverEgypt.Application.Mapping;
using DiscoverEgypt.Application.Responses.Earnings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Tests;

public sealed class HostEarningsResponseTests
{
    [Fact]
    public async Task Handler_ReturnsResponseAndScopesPayoutReadToRequestedHost()
    {
        var scheduledStart = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var eligibleAt = scheduledStart.AddDays(1);
        var payout = new Payout
        {
            Id = 9,
            BookingId = 27,
            Amount = 425m,
            EligibleAt = eligibleAt,
            Status = PayoutStatus.Pending,
            Booking = new Booking
            {
                ScheduledStart = scheduledStart,
                Experience = new Experience { Title = "Traditional Cooking" }
            }
        };
        var repository = new FakeHostEarningsRepository([payout]);
        using var loggerFactory = LoggerFactory.Create(_ => { });
        var mapper = new MapperConfiguration(
            configuration => configuration.AddProfile<HostEarningsMappingProfile>(),
            loggerFactory).CreateMapper();
        var handler = new GetHostEarningsQueryHandler(
            repository,
            mapper,
            new GetHostEarningsQueryValidator());

        var response = await handler.Handle(
            new GetHostEarningsQuery("host-123", PayoutStatus.Pending, Page: 2, PageSize: 5),
            CancellationToken.None);

        Assert.Equal("host-123", repository.RequestedHostId);
        Assert.Equal(PayoutStatus.Pending, repository.RequestedStatus);
        Assert.Equal(2, repository.RequestedPage);
        Assert.Equal(5, repository.RequestedPageSize);
        Assert.Equal(425m, response.PendingAmount);
        Assert.Equal(900m, response.PaidAmount);
        Assert.Equal(25m, response.CancelledAmount);
        Assert.Equal(7, response.TotalCount);
        Assert.Equal(2, response.Page);
        Assert.Equal(5, response.PageSize);
        Assert.Equal(PayoutStatus.Pending, response.SelectedStatus);
        Assert.Equal(1325m, response.TotalEarnings);
        Assert.Equal(2, response.TotalPages);

        var item = Assert.Single(response.Payouts);
        Assert.Equal(9, item.PayoutId);
        Assert.Equal(27, item.BookingId);
        Assert.Equal("Traditional Cooking", item.ExperienceTitle);
        Assert.Equal(scheduledStart, item.ScheduledStart);
        Assert.Equal(425m, item.Amount);
        Assert.Equal(PayoutStatus.Pending, item.Status);
        Assert.Equal(eligibleAt, item.EligibleAt);
    }

    private sealed class FakeHostEarningsRepository(IReadOnlyList<Payout> payouts) : IHostEarningsRepository
    {
        public string? RequestedHostId { get; private set; }
        public PayoutStatus? RequestedStatus { get; private set; }
        public int RequestedPage { get; private set; }
        public int RequestedPageSize { get; private set; }

        public Task<IReadOnlyList<Payout>> GetForHostAsync(
            string userId,
            PayoutStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            RequestedHostId = userId;
            RequestedStatus = status;
            RequestedPage = page;
            RequestedPageSize = pageSize;
            return Task.FromResult(payouts);
        }

        public Task<int> GetCountAsync(
            string userId,
            PayoutStatus? status,
            CancellationToken cancellationToken = default) => Task.FromResult(7);

        public Task<IReadOnlyDictionary<PayoutStatus, decimal>> GetTotalsAsync(
            string userId,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyDictionary<PayoutStatus, decimal>>(
                new Dictionary<PayoutStatus, decimal>
                {
                    [PayoutStatus.Pending] = 425m,
                    [PayoutStatus.Paid] = 900m,
                    [PayoutStatus.Cancelled] = 25m
                });
    }
}
