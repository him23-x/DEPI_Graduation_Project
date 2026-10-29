
using DiscoverEgypt.Domain.Enums.Bookings;

namespace DiscoverEgypt.Application.DTOs.Bookings;

public sealed record HostBookingListItemDto(
    int Id,
    string TouristName,
    string ExperienceTitle,
    DateTime ScheduledStart,
    DateTime ScheduledEnd,
    int NumberOfGuests,
    decimal ServiceAmount,
    BookingStatus Status,
    DateTime? ResponseDeadline);

