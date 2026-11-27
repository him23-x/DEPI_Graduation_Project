using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public interface ITouristPaymentRepository
    {
        Task<MockPaymentResponse?> GetOwnedPaymentAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken = default);

        Task<bool> SaveStripeSessionAsync(
            string userId,
            int bookingId,
            string stripeSessionId,
            CancellationToken cancellationToken = default);

        Task<bool> TryCompletePendingPaymentAsync(
            string userId,
            int bookingId,
            PaymentStatus status,
            DateTime? paidAtUtc,
            string? stripeSessionId = null,
            string? stripePaymentIntentId = null,
            CancellationToken cancellationToken = default);

        Task<bool> TryCompletePaymentFromWebhookAsync(
            int bookingId,
            PaymentStatus status,
            DateTime? paidAtUtc,
            string? stripeSessionId,
            string? stripePaymentIntentId,
            CancellationToken cancellationToken = default);
    }
}
