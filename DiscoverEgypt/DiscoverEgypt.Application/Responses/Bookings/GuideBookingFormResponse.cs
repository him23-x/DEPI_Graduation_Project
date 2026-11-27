using DiscoverEgypt.Application.DTOs.Bookings;

namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record GuideBookingFormResponse(
        GuideBookingDataDto Guide,
        GuideBookingInputDto Form,
        DateTime MinimumStartLocal);
}
