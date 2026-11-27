using AutoMapper;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Dashboard.Handlers.Queries;
using DiscoverEgypt.Application.Features.Dashboard.Requests.Queries;
using DiscoverEgypt.Application.Features.Dashboard.Validators;
using DiscoverEgypt.Application.Mapping;
using DiscoverEgypt.Application.Responses.Dashboard;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Experiences;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Tests;

public sealed class HostDashboardResponseTests
{
    [Fact]
    public async Task Handler_ProjectsCurrentDashboardAndScopesEveryRepositoryCallToHost()
    {
        var now = new DateTimeOffset(2026, 9, 14, 10, 0, 0, TimeSpan.Zero);
        var pending = new Booking
        {
            Id = 12,
            ScheduledStart = now.AddDays(2).UtcDateTime,
            Experience = new Experience { Title = "Pottery Workshop" }
        };
        var upcoming = new Booking
        {
            Id = 13,
            ScheduledStart = now.AddDays(5).UtcDateTime,
            Experience = new Experience { Title = "Old Cairo Walk" }
        };
        var repository = new FakeHostDashboardRepository(pending, upcoming);
        using var loggerFactory = LoggerFactory.Create(_ => { });
        var mapper = new MapperConfiguration(
            configuration => configuration.AddProfile<HostDashboardMappingProfile>(),
            loggerFactory).CreateMapper();
        var handler = new GetHostDashboardQueryHandler(
            repository,
            new FixedTimeProvider(now),
            mapper,
            new GetHostDashboardQueryValidator());

        var result = await handler.Handle(new GetHostDashboardQuery("host-456"), CancellationToken.None);

        Assert.NotEmpty(repository.RequestedHostIds);
        Assert.All(repository.RequestedHostIds, userId => Assert.Equal("host-456", userId));
        Assert.Equal(now.UtcDateTime, repository.UpcomingBookingsFrom);
        Assert.Equal(3, repository.PendingRequestLimit);
        Assert.Equal(3, repository.UpcomingBookingLimit);
        Assert.Equal(4, result.PendingRequestCount);
        Assert.Equal(6, result.ConfirmedBookingCount);
        Assert.Equal(100m, result.PendingPayoutAmount);
        Assert.Equal(200m, result.PaidPayoutAmount);
        Assert.Equal(5m, result.CancelledPayoutAmount);
        Assert.Equal(2, result.UnreadNotificationCount);
        Assert.Equal(4.5m, result.AverageRating);
        Assert.Equal(3, result.ReviewCount);

        Assert.Equal(1, result.ExperienceStatuses.Single(item => item.Status == ExperienceStatus.Draft).Count);
        Assert.Equal(2, result.ExperienceStatuses.Single(item => item.Status == ExperienceStatus.Published).Count);
        Assert.Equal(0, result.ExperienceStatuses.Single(item => item.Status == ExperienceStatus.Unpublished).Count);

        var pendingItem = Assert.Single(result.NearestPendingRequests);
        Assert.Equal(12, pendingItem.BookingId);
        Assert.Equal("Pottery Workshop", pendingItem.ExperienceTitle);
        var upcomingItem = Assert.Single(result.UpcomingConfirmedBookings);
        Assert.Equal(13, upcomingItem.BookingId);
        Assert.Equal("Old Cairo Walk", upcomingItem.ExperienceTitle);
    }

    private sealed class FakeHostDashboardRepository(Booking pending, Booking upcoming) : IHostDashboardRepository
    {
        public List<string> RequestedHostIds { get; } = [];
        public DateTime? UpcomingBookingsFrom { get; private set; }
        public int PendingRequestLimit { get; private set; }
        public int UpcomingBookingLimit { get; private set; }

        public Task<IReadOnlyList<Experience>> GetExperiencesAsync(string userId, CancellationToken cancellationToken = default)
        {
            RequestedHostIds.Add(userId);
            return Task.FromResult<IReadOnlyList<Experience>>(
            [
                new Experience { Status = ExperienceStatus.Draft },
                new Experience { Status = ExperienceStatus.Published },
                new Experience { Status = ExperienceStatus.Published }
            ]);
        }

        public Task<int> GetPendingRequestCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            RequestedHostIds.Add(userId);
            return Task.FromResult(4);
        }

        public Task<IReadOnlyList<Booking>> GetPendingRequestsAsync(string userId, int limit, CancellationToken cancellationToken = default)
        {
            RequestedHostIds.Add(userId);
            PendingRequestLimit = limit;
            return Task.FromResult<IReadOnlyList<Booking>>([pending]);
        }

        public Task<int> GetConfirmedBookingCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            RequestedHostIds.Add(userId);
            return Task.FromResult(6);
        }

        public Task<IReadOnlyList<Booking>> GetUpcomingConfirmedBookingsAsync(string userId, DateTime from, int limit, CancellationToken cancellationToken = default)
        {
            RequestedHostIds.Add(userId);
            UpcomingBookingsFrom = from;
            UpcomingBookingLimit = limit;
            return Task.FromResult<IReadOnlyList<Booking>>([upcoming]);
        }

        public Task<IReadOnlyDictionary<PayoutStatus, decimal>> GetPayoutTotalsAsync(string userId, CancellationToken cancellationToken = default)
        {
            RequestedHostIds.Add(userId);
            return Task.FromResult<IReadOnlyDictionary<PayoutStatus, decimal>>(
                new Dictionary<PayoutStatus, decimal>
                {
                    [PayoutStatus.Pending] = 100m,
                    [PayoutStatus.Paid] = 200m,
                    [PayoutStatus.Cancelled] = 5m
                });
        }

        public Task<int> GetReviewCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            RequestedHostIds.Add(userId);
            return Task.FromResult(3);
        }

        public Task<decimal> GetAverageRatingAsync(string userId, CancellationToken cancellationToken = default)
        {
            RequestedHostIds.Add(userId);
            return Task.FromResult(4.5m);
        }

        public Task<int> GetUnreadNotificationCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            RequestedHostIds.Add(userId);
            return Task.FromResult(2);
        }
    }

    private sealed class FixedTimeProvider(DateTimeOffset currentTime) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => currentTime;
    }
}
