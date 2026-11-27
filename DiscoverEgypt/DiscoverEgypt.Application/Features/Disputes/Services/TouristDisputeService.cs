using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Disputes;
using DiscoverEgypt.Application.Features.Disputes.Policies;
using DiscoverEgypt.Application.Responses.Disputes;

namespace DiscoverEgypt.Application.Features.Disputes.Services
{
    public sealed class TouristDisputeService
    {
        private readonly ITouristDisputeRepository _repository;
        private readonly TimeProvider _timeProvider;

        public TouristDisputeService(ITouristDisputeRepository repository, TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<TouristDisputeBookingResponse?> GetEligibleBookingAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            if (bookingId <= 0) return null;

            var booking = await _repository.GetOwnedBookingAsync(userId, bookingId, cancellationToken);
            return booking is not null && IsEligible(booking) ? booking : null;
        }

        public async Task<TouristDisputeOpenResponse> OpenAsync(
            string userId,
            int bookingId,
            string description,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            if (bookingId <= 0) return new TouristDisputeOpenResponse(DisputeOpenOutcome.BookingUnavailable, null);

            var booking = await _repository.GetOwnedBookingAsync(userId, bookingId, cancellationToken);
            if (booking is null)
                return new TouristDisputeOpenResponse(DisputeOpenOutcome.BookingUnavailable, null);
            if (!IsEligible(booking))
                return new TouristDisputeOpenResponse(DisputeOpenOutcome.BookingNotEligible, null);

            var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
            return await _repository.OpenAsync(
                userId,
                bookingId,
                description.Trim(),
                nowUtc,
                cancellationToken);
        }

        public Task<IReadOnlyList<TouristDisputeListItemResponse>> GetForTouristAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            return _repository.GetForTouristAsync(userId, cancellationToken);
        }

        public Task<TouristDisputeDetailsResponse?> GetDetailsAsync(
            string userId,
            int disputeId,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            if (disputeId <= 0) return Task.FromResult<TouristDisputeDetailsResponse?>(null);
            return _repository.GetDetailsForTouristAsync(userId, disputeId, cancellationToken);
        }

        private bool IsEligible(TouristDisputeBookingResponse booking) =>
            TouristDisputeEligibilityPolicy.CanOpen(
                booking.BookingStatus,
                booking.ScheduledEndUtc,
                booking.PayoutAmount,
                _timeProvider.GetUtcNow().UtcDateTime);
    }
}
