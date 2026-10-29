namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IBookingLifecycleService
{
    Task CancelForNoResponseAsync(DateTime now, CancellationToken cancellationToken = default);

    Task CancelUnpaidBookingsAsync(DateTime now, CancellationToken cancellationToken = default);

    Task AutoCompleteAsync(DateTime now, CancellationToken cancellationToken = default);

    Task ReleaseEligiblePayoutsAsync(DateTime now, CancellationToken cancellationToken = default);

    Task ExpireGuideLicensesAsync(DateTime now, CancellationToken cancellationToken = default);
}
