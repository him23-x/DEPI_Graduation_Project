using DiscoverEgypt.Application.DTOs.Bookings;

using DiscoverEgypt.Application.Enums.Bookings;

using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Features.Bookings.Policies;

public static class HostBookingPolicy
{
    public static DateTime? GetResponseDeadline(DateTime? paidAt, TimeSpan responseTimeout) =>
        paidAt?.Add(responseTimeout);

    public static HostBookingActionOutcome? GetAcceptFailure(
        BookingStatus status,
        PaymentStatus paymentStatus,
        DateTime? paidAt,
        bool isProviderSuspended,
        bool hasOverlap,
        bool payoutExists,
        DateTime now,
        TimeSpan responseTimeout)
    {
        if (status != BookingStatus.Pending) return HostBookingActionOutcome.NotPending;
        if (paymentStatus != PaymentStatus.Paid || !paidAt.HasValue) return HostBookingActionOutcome.PaymentNotPaid;
        if (isProviderSuspended) return HostBookingActionOutcome.ProviderSuspended;
        if (now > paidAt.Value.Add(responseTimeout)) return HostBookingActionOutcome.ResponseWindowExpired;
        if (hasOverlap) return HostBookingActionOutcome.Overlap;
        if (payoutExists) return HostBookingActionOutcome.PayoutAlreadyExists;
        return null;
    }

    public static HostBookingActionOutcome? GetRejectFailure(BookingStatus status, PaymentStatus paymentStatus, decimal serviceAmount)
    {
        if (status != BookingStatus.Pending) return HostBookingActionOutcome.NotPending;
        if (paymentStatus != PaymentStatus.Paid) return HostBookingActionOutcome.PaymentNotPaid;
        if (serviceAmount <= 0) return HostBookingActionOutcome.InvalidRefundAmount;
        return null;
    }

    public static HostBookingActionOutcome? GetCompleteFailure(
        BookingStatus status,
        DateTime scheduledEnd,
        bool hasOpenDispute,
        DateTime now)
    {
        if (status != BookingStatus.Confirmed) return HostBookingActionOutcome.NotConfirmed;
        if (now < scheduledEnd) return HostBookingActionOutcome.NotEnded;
        if (hasOpenDispute) return HostBookingActionOutcome.OpenDispute;
        return null;
    }

    public static bool IsInTab(BookingStatus status, HostBookingTab tab) => tab switch
    {
        HostBookingTab.Pending => status == BookingStatus.Pending,
        HostBookingTab.Confirmed => status == BookingStatus.Confirmed,
        HostBookingTab.Completed => status == BookingStatus.Completed,
        HostBookingTab.RejectedCancelled => status is BookingStatus.Rejected or BookingStatus.Cancelled,
        _ => false
    };
}
