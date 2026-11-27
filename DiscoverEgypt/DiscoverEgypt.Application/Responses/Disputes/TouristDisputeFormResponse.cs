using DiscoverEgypt.Application.DTOs.Disputes;

namespace DiscoverEgypt.Application.Responses.Disputes
{
    public sealed record TouristDisputeFormResponse(
        TouristDisputeBookingResponse Booking,
        OpenTouristDisputeDto Form);
}
