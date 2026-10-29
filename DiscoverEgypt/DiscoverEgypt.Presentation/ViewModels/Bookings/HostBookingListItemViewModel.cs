
using DiscoverEgypt.Domain.Enums.Bookings;

namespace DiscoverEgypt.Presentation.ViewModels.Bookings;

public sealed record HostBookingListItemViewModel(
    int Id,
    string TouristName,
    string ExperienceTitle,
    DateTime ScheduledStart,
    DateTime ScheduledEnd,
    int NumberOfGuests,
    decimal ServiceAmount,
    BookingStatus Status,
    DateTime? ResponseDeadline);
