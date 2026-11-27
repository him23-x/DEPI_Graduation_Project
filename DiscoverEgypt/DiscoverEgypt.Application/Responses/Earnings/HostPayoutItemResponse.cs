using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Responses.Earnings
{
    public sealed record HostPayoutItemResponse(
        int PayoutId,
        int BookingId,
        string ExperienceTitle,
        DateTime ScheduledStart,
        decimal Amount,
        PayoutStatus Status,
        DateTime EligibleAt,
        DateTime? PaidAt);
}
