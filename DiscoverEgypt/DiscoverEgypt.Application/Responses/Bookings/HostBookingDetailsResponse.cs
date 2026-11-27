namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record HostBookingDetailsResponse
    {
        public required HostBookingDetailResponse Booking { get; init; }
        public string? Notice { get; init; }
        public string? Error { get; init; }
    }
}
