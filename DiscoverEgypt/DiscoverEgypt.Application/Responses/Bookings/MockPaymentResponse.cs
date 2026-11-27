using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record MockPaymentResponse(
        int BookingId,
        BookingType BookingType,
        string ServiceTitle,
        DateTime ScheduledStartUtc,
        decimal Amount,
        BookingStatus BookingStatus,
        PaymentStatus PaymentStatus,
        DateTime? PaidAtUtc,
        string? StripeSessionId = null);
}
