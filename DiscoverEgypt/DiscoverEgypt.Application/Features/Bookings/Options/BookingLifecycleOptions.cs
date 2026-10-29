namespace DiscoverEgypt.Application.Features.Bookings.Options;

public sealed class BookingLifecycleOptions
{
    public TimeSpan ProviderResponseTimeout { get; set; } = TimeSpan.FromHours(12);
    public TimeSpan UnpaidBookingTimeout { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan LifecyclePollingInterval { get; set; } = TimeSpan.FromMinutes(1);
}
