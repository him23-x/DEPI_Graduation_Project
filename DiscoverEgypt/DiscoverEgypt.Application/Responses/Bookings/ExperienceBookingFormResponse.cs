using DiscoverEgypt.Application.DTOs.Bookings;

namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record ExperienceBookingFormResponse(
        ExperienceBookingDataDto Experience,
        ExperienceBookingInputDto Form,
        DateTime MinimumStartLocal);
}
