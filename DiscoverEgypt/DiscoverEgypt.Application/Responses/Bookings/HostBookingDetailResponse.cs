using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record HostBookingDetailResponse(
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
        bool HasOpenDispute)
    {
        public bool IsProviderSuspended { get; init; }
        public bool HasOverlap { get; init; }
        public bool HasPayout { get; init; }
        public bool CanAccept { get; init; }
        public bool CanReject { get; init; }
        public bool CanComplete { get; init; }
    }
}
