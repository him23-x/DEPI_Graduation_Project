using DiscoverEgypt.Domain.Enums.Bookings;

namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record HostBookingListItemResponse(
        int Id,
        string TouristName,
        string ExperienceTitle,
        DateTime ScheduledStart,
        DateTime ScheduledEnd,
        int NumberOfGuests,
        decimal ServiceAmount,
        BookingStatus Status)
    {
        public DateTime? PaidAt { get; init; }
        public DateTime? ResponseDeadline { get; init; }
    }
}
