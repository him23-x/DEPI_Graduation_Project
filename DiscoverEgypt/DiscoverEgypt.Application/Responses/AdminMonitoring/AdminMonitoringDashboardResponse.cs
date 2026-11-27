namespace DiscoverEgypt.Application.Responses.AdminMonitoring
{
    public sealed record AdminMonitoringDashboardResponse(
        DateTime GeneratedAtUtc,
        IReadOnlyList<AdminMonitoringStatusResponse> Bookings,
        IReadOnlyList<AdminMonitoringStatusResponse> Payments,
        IReadOnlyList<AdminMonitoringStatusResponse> Payouts)
    {
        public int BookingCount => Bookings.Sum(item => item.RecordCount);
        public decimal BookingAmount => Bookings.Sum(item => item.Amount);
        public int PaymentCount => Payments.Sum(item => item.RecordCount);
        public decimal PaymentAmount => Payments.Sum(item => item.Amount);
        public int PayoutCount => Payouts.Sum(item => item.RecordCount);
        public decimal PayoutAmount => Payouts.Sum(item => item.Amount);
    }
}
