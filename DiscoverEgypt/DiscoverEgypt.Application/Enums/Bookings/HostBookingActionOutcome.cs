using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Application.Enums.Bookings;

public enum HostBookingActionOutcome
{
    Success, BookingUnavailable, NotPending, PaymentNotPaid, ResponseWindowExpired,
    ProviderSuspended, Overlap, ConcurrencyUnavailable, PayoutAlreadyExists,
    InvalidRefundAmount, NotConfirmed, NotEnded, OpenDispute
}
