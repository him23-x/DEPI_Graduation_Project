
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.DTOs.Bookings;

public sealed record HostBookingDetailDto(
    int Id,
    string TouristName,
    string TouristEmail,
    string ExperienceTitle,
    DateTime ScheduledStart,
    DateTime ScheduledEnd,
    int NumberOfGuests,
    decimal ServiceAmount,
    decimal PlatformFeeAmount,
    decimal TotalAmount,
    BookingStatus Status,
    PaymentStatus PaymentStatus,
    DateTime? PaidAt,
    DateTime? ResponseDeadline,
    string MeetingPointArea,
    string ExactMeetingPoint,
    decimal? PayoutAmount,
    DateTime? PayoutEligibleAt,
    PayoutStatus? PayoutStatus,
    bool HasOpenDispute,
    bool CanAccept,
    bool CanReject,
    bool CanComplete);

