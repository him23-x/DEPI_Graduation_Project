using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record TouristBookingDetailsResponse(
        int BookingId,
        BookingType BookingType,
        string Title,
        string? CulturalArea,
        string? Destination,
        DateTime ScheduledStartUtc,
        DateTime ScheduledEndUtc,
        int? NumberOfGuests,
        int? GuideHours,
        decimal ServiceAmount,
        decimal PlatformFeeRate,
        decimal PlatformFeeAmount,
        decimal TotalAmount,
        BookingStatus BookingStatus,
        PaymentStatus PaymentStatus,
        decimal PaymentAmount,
        decimal RefundedAmount,
        IReadOnlyList<TouristBookingRefundResponse> Refunds,
        decimal? PayoutAmount,
        PayoutStatus? PayoutStatus,
        DateTime? PayoutEligibleAtUtc,
        bool HasOpenDispute,
        bool CanCancel,
        bool CanOpenDispute,
        bool CanReview,
        int? ReviewRating,
        string? ReviewComment,
        DateTime? ReviewCreatedAtUtc,
        bool CanShareBooking,
        string? ExactMeetingPoint);
}
