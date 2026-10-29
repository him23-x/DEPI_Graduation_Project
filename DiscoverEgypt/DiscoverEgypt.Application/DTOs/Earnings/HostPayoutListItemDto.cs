using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.DTOs.Earnings;

public sealed record HostPayoutListItemDto(
    int PayoutId,
    int BookingId,
    string ExperienceTitle,
    DateTime ScheduledStart,
    decimal Amount,
    PayoutStatus Status,
    DateTime EligibleAt,
    DateTime? PaidAt);
