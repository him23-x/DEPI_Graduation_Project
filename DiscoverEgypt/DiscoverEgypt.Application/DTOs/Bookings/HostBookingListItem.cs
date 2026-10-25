using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.Bookings;

public sealed record HostBookingListItem(
    int Id,
    string TouristName,
    string ExperienceTitle,
    DateTime ScheduledStart,
    DateTime ScheduledEnd,
    int NumberOfGuests,
    decimal ServiceAmount,
    BookingStatus Status,
    DateTime? ResponseDeadline);

