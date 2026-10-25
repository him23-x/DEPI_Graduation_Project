using DiscoverEgypt.Application.DTOs.Bookings;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostBookingRepository
{
    Task<IReadOnlyList<HostBookingListItem>> GetForHostAsync(
        string userId,
        HostBookingTab tab,
        CancellationToken cancellationToken = default);

    Task<HostBookingDetails?> GetDetailsAsync(
        string userId,
        int bookingId,
        CancellationToken cancellationToken = default);

    Task<HostBookingActionResult> AcceptAsync(
        string userId,
        int bookingId,
        DateTime now,
        CancellationToken cancellationToken = default);

    Task<HostBookingActionResult> RejectAsync(
        string userId,
        int bookingId,
        DateTime now,
        CancellationToken cancellationToken = default);

    Task<HostBookingActionResult> CompleteAsync(
        string userId,
        int bookingId,
        DateTime now,
        CancellationToken cancellationToken = default);
}
