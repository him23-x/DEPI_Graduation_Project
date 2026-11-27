using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Responses.AdminMonitoring;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Admin.Monitoring
{
    public sealed class AdminMonitoringRepository : IAdminMonitoringRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public AdminMonitoringRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public async Task<AdminMonitoringDashboardResponse> GetSummaryAsync(
            CancellationToken cancellationToken = default)
        {
            var bookings = await _dbContext.Bookings.AsNoTracking()
                .GroupBy(booking => booking.Status)
                .Select(group => new
                {
                    Status = group.Key,
                    RecordCount = group.Count(),
                    Amount = group.Sum(booking => booking.TotalAmount)
                })
                .ToListAsync(cancellationToken);

            var payments = await _dbContext.Payments.AsNoTracking()
                .GroupBy(payment => payment.Status)
                .Select(group => new
                {
                    Status = group.Key,
                    RecordCount = group.Count(),
                    Amount = group.Sum(payment => payment.Amount)
                })
                .ToListAsync(cancellationToken);

            var payouts = await _dbContext.Payouts.AsNoTracking()
                .GroupBy(payout => payout.Status)
                .Select(group => new
                {
                    Status = group.Key,
                    RecordCount = group.Count(),
                    Amount = group.Sum(payout => payout.Amount)
                })
                .ToListAsync(cancellationToken);

            return new AdminMonitoringDashboardResponse(
                DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
                bookings.Select(item => new AdminMonitoringStatusResponse(
                    item.Status.ToString(), item.RecordCount, item.Amount)).OrderBy(item => item.Status).ToArray(),
                payments.Select(item => new AdminMonitoringStatusResponse(
                    item.Status.ToString(), item.RecordCount, item.Amount)).OrderBy(item => item.Status).ToArray(),
                payouts.Select(item => new AdminMonitoringStatusResponse(
                    item.Status.ToString(), item.RecordCount, item.Amount)).OrderBy(item => item.Status).ToArray());
        }
    }
}
