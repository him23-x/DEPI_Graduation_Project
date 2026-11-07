using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public sealed class HostBookingActionContext
    {
        public Booking? Booking { get; init; }
        public DateTime Now { get; init; }
        public bool IsProviderSuspended { get; init; }
        public bool HasOverlap { get; init; }
        public bool HasOpenDispute { get; init; }
        public bool HasPayout { get; init; }
        public Action<Payout> AddPayout { get; init; } = _ => { };
        public Action<Refund> AddRefund { get; init; } = _ => { };
    }
}
