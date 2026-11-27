using DiscoverEgypt.Application.Enums.Bookings;
using DiscoverEgypt.Application.Responses.Accounts;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface IHostBookingRepository
    {
        Task<IReadOnlyList<Booking>> GetForHostAsync(
            string userId,
            HostBookingTab tab,
            CancellationToken cancellationToken = default);

        Task<Booking?> GetDetailsAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<string, UserSummaryResponse>> GetTouristsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);

        Task<bool> IsProviderSuspendedAsync(
            string userId,
            DateTime bookingCreatedAtUtc,
            DateTime nowUtc,
            CancellationToken cancellationToken = default);

        Task<bool> HasConfirmedOverlapAsync(string userId, DateTime scheduledStart, DateTime scheduledEnd, int exceptBookingId, CancellationToken cancellationToken = default);

        Task<HostBookingActionOutcome> ExecuteActionAsync(
            string userId,
            int bookingId,
            Func<HostBookingActionContext, CancellationToken, Task<HostBookingActionOutcome>> action,
            CancellationToken cancellationToken = default);
    }
}

