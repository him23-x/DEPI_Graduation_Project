using DiscoverEgypt.Domain.Enums.Bookings;

namespace DiscoverEgypt.Application.Responses.BookingShares
{
    public sealed record SharedBookingResponse(
        BookingType BookingType,
        string ServiceName,
        string ProviderName,
        DateTime ScheduledStartUtc,
        DateTime ScheduledEndUtc,
        string? PublicArea,
        string Location);
}
