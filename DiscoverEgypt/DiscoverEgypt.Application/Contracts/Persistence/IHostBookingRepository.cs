using DiscoverEgypt.Application.DTOs.Bookings;

using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostBookingRepository
{
    Task<IReadOnlyList<HostBookingListItemDto>> GetForHostAsync(
        string userId,
        HostBookingTab tab,
        CancellationToken cancellationToken = default);

    Task<HostBookingDetailDto?> GetDetailsAsync(
        string userId,
        int bookingId,
        CancellationToken cancellationToken = default);

    Task<HostBookingActionOutcome> AcceptAsync(
        string userId,
        int bookingId,
        DateTime now,
        CancellationToken cancellationToken = default);

    Task<HostBookingActionOutcome> RejectAsync(
        string userId,
        int bookingId,
        DateTime now,
        CancellationToken cancellationToken = default);

    Task<HostBookingActionOutcome> CompleteAsync(
        string userId,
        int bookingId,
        DateTime now,
        CancellationToken cancellationToken = default);
}
