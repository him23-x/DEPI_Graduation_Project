namespace DiscoverEgypt.Application.Bookings;

// NOTE (sample only): TouristId comes from the request body.
// In the real project it comes from the logged-in user (ASP.NET Core Identity), never from the client.
public record CreateExperienceBookingRequest(
    Guid TouristId,
    Guid ExperienceId,
    int NumberOfGuests,
    DateTime StartAt);
