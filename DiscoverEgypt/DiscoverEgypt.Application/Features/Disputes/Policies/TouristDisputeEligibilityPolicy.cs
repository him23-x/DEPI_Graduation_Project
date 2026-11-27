using DiscoverEgypt.Application.Responses.Disputes;
using DiscoverEgypt.Domain.Enums.Bookings;

namespace DiscoverEgypt.Application.Features.Disputes.Policies
{
    public static class TouristDisputeEligibilityPolicy
    {
        private static readonly TimeSpan ComplaintWindow = TimeSpan.FromHours(24);

        public static bool CanOpen(
            BookingStatus bookingStatus,
            DateTime scheduledEndUtc,
            decimal? providerAllocation,
            DateTime nowUtc)
        {
            var endUtc = AsUtc(scheduledEndUtc);
            var currentUtc = AsUtc(nowUtc);

            if (bookingStatus is BookingStatus.Confirmed or BookingStatus.Completed)
                return currentUtc <= endUtc.Add(ComplaintWindow);

            return bookingStatus == BookingStatus.Cancelled && providerAllocation.GetValueOrDefault() > 0m;
        }

        private static DateTime AsUtc(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}
