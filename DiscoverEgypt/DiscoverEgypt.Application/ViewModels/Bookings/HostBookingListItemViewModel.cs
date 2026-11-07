
using DiscoverEgypt.Domain.Enums.Bookings;

namespace DiscoverEgypt.Application.ViewModels.Bookings
{
    public sealed record HostBookingListItemViewModel(
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

