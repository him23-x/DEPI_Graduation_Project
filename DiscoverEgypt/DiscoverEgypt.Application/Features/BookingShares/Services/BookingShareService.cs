using System.Security.Cryptography;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.BookingShares;
using DiscoverEgypt.Application.Responses.BookingShares;

namespace DiscoverEgypt.Application.Features.BookingShares.Services
{
    public sealed class BookingShareService
    {
        private readonly ITouristBookingShareRepository _repository;
        private readonly TimeProvider _timeProvider;

        public BookingShareService(ITouristBookingShareRepository repository, TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public Task<BookingShareCreateResponse> CreateAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            if (bookingId <= 0)
                return Task.FromResult(new BookingShareCreateResponse(
                    BookingShareCreateOutcome.BookingUnavailable, null, null));

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
            return _repository.CreateAsync(
                userId,
                bookingId,
                token,
                _timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken);
        }

        public async Task<SharedBookingResponse?> GetSharedAsync(string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token) || token.Length > 256) return null;
            var booking = await _repository.GetActiveAsync(
                token,
                _timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken);
            return booking is null
                ? null
                : booking with
                {
                    ScheduledStartUtc = DateTime.SpecifyKind(booking.ScheduledStartUtc, DateTimeKind.Utc),
                    ScheduledEndUtc = DateTime.SpecifyKind(booking.ScheduledEndUtc, DateTimeKind.Utc)
                };
        }
    }
}
