namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record TouristBookingRefundResponse(decimal Amount, string Reason, DateTime CreatedAtUtc);
}
