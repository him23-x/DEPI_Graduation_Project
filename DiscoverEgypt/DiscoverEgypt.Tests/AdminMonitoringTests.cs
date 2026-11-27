using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.AdminMonitoring.Handlers.Queries;
using DiscoverEgypt.Application.Features.AdminMonitoring.Requests.Queries;
using DiscoverEgypt.Application.Responses.AdminMonitoring;
using DiscoverEgypt.Presentation.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;

namespace DiscoverEgypt.Tests
{
    public sealed class AdminMonitoringTests
    {
        [Fact]
        public async Task Handler_ReturnsStoredStatusBreakdownsWithUtcGenerationTime()
        {
            var repository = new FakeRepository
            {
                Summary = new AdminMonitoringDashboardResponse(
                    DateTime.MinValue,
                    [new("Confirmed", 2, 1200m), new("Completed", 1, 450m)],
                    [new("Paid", 2, 1380m), new("Refunded", 1, 270m)],
                    [new("Pending", 1, 300m), new("Paid", 1, 600m)])
            };
            var generatedAt = new DateTimeOffset(2026, 11, 24, 13, 40, 0, TimeSpan.Zero);
            var handler = new GetAdminMonitoringDashboardQueryHandler(repository, new FixedTimeProvider(generatedAt));

            var result = await handler.Handle(new GetAdminMonitoringDashboardQuery(), CancellationToken.None);

            Assert.Equal(generatedAt.UtcDateTime, result.GeneratedAtUtc);
            Assert.Equal(DateTimeKind.Utc, result.GeneratedAtUtc.Kind);
            Assert.Equal(3, result.BookingCount);
            Assert.Equal(1650m, result.BookingAmount);
            Assert.Equal(3, result.PaymentCount);
            Assert.Equal(1650m, result.PaymentAmount);
            Assert.Equal(2, result.PayoutCount);
            Assert.Equal(900m, result.PayoutAmount);
            Assert.Equal("Confirmed", result.Bookings[0].Status);
        }

        [Fact]
        public async Task Handler_ReturnsZeroTotalsForEmptyData()
        {
            var handler = new GetAdminMonitoringDashboardQueryHandler(
                new FakeRepository(), new FixedTimeProvider(DateTimeOffset.UtcNow));

            var result = await handler.Handle(new GetAdminMonitoringDashboardQuery(), CancellationToken.None);

            Assert.Equal(0, result.BookingCount);
            Assert.Equal(0m, result.BookingAmount);
            Assert.Equal(0, result.PaymentCount);
            Assert.Equal(0m, result.PaymentAmount);
            Assert.Equal(0, result.PayoutCount);
            Assert.Equal(0m, result.PayoutAmount);
        }

        [Fact]
        public void MonitoringPage_RequiresAdminRole()
        {
            var attribute = typeof(MonitoringController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                .Cast<AuthorizeAttribute>().Single();

            Assert.Equal("Admin", attribute.Roles);
        }

        private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
        }

        private sealed class FakeRepository : IAdminMonitoringRepository
        {
            public AdminMonitoringDashboardResponse Summary { get; init; } = new(
                DateTime.MinValue, [], [], []);

            public Task<AdminMonitoringDashboardResponse> GetSummaryAsync(
                CancellationToken cancellationToken = default) => Task.FromResult(Summary);
        }
    }
}
