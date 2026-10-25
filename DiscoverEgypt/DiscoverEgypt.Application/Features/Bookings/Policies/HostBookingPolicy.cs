using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.Features.Bookings.Policies;

public static class HostBookingPolicy
{
    public static DateTime? GetResponseDeadline(DateTime? paidAt) => paidAt?.AddHours(12);

    public static HostBookingActionResult? GetAcceptFailure(
        BookingStatus status,
        PaymentStatus paymentStatus,
        DateTime? paidAt,
        bool isProviderSuspended,
        bool hasOverlap,
        bool payoutExists,
        DateTime now)
    {
        if (status != BookingStatus.Pending) return HostBookingActionResult.NotPending;
        if (paymentStatus != PaymentStatus.Paid || !paidAt.HasValue) return HostBookingActionResult.PaymentNotPaid;
        if (isProviderSuspended) return HostBookingActionResult.ProviderSuspended;
        if (now > paidAt.Value.AddHours(12)) return HostBookingActionResult.ResponseWindowExpired;
        if (hasOverlap) return HostBookingActionResult.Overlap;
        if (payoutExists) return HostBookingActionResult.PayoutAlreadyExists;
        return null;
    }

    public static HostBookingActionResult? GetRejectFailure(BookingStatus status, PaymentStatus paymentStatus, decimal serviceAmount)
    {
        if (status != BookingStatus.Pending) return HostBookingActionResult.NotPending;
        if (paymentStatus != PaymentStatus.Paid) return HostBookingActionResult.PaymentNotPaid;
        if (serviceAmount <= 0) return HostBookingActionResult.InvalidRefundAmount;
        return null;
    }

    public static HostBookingActionResult? GetCompleteFailure(
        BookingStatus status,
        DateTime scheduledEnd,
        bool hasOpenDispute,
        DateTime now)
    {
        if (status != BookingStatus.Confirmed) return HostBookingActionResult.NotConfirmed;
        if (now < scheduledEnd) return HostBookingActionResult.NotEnded;
        if (hasOpenDispute) return HostBookingActionResult.OpenDispute;
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
