using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Notifications;
using DiscoverEgypt.Application.Features.Bookings.Policies;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Notifications;
using DiscoverEgypt.Domain.Enums.Payments;
using MediatR;
using DiscoverEgypt.Application.Enums.Bookings;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands
{
    public sealed class RejectHostBookingCommandHandler : IRequestHandler<RejectHostBookingCommand, HostBookingActionOutcome>
    {
        private readonly IHostBookingRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly IValidator<RejectHostBookingCommand> _validator;

        public RejectHostBookingCommandHandler(IHostBookingRepository repository, INotificationService notificationService, IValidator<RejectHostBookingCommand> validator)
        {
            _repository = repository;
            _notificationService = notificationService;
            _validator = validator;
        }

        public async Task<HostBookingActionOutcome> Handle(RejectHostBookingCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            return await _repository.ExecuteActionAsync(request.UserId, request.BookingId, async (context, token) =>
            {
                var booking = context.Booking;
                if (booking is null) return HostBookingActionOutcome.BookingUnavailable;

                var failure = HostBookingPolicy.GetRejectFailure(
                    booking.Status,
                    booking.Payment.Status,
                    booking.ServiceAmount);
                if (failure.HasValue) return failure.Value;
                if (context.HasPayout) return HostBookingActionOutcome.PayoutAlreadyExists;

                booking.Status = BookingStatus.Rejected;
                booking.Payment.Status = PaymentStatus.Refunded;
                context.AddRefund(new Refund
                {
                    BookingId = booking.Id,
                    Amount = booking.ServiceAmount,
                    Reason = "Host rejected the booking. The service amount is refunded; the platform fee is not refundable.",
                    CreatedAt = context.Now
                });
                await _notificationService.CreateAsync(
                    booking.TouristUserId,
                    NotificationType.BookingStatusUpdate,
                    $"Your booking for {booking.Experience!.Title} was declined. The service amount will be refunded; the platform fee is not refundable.",
                    booking.Id,
                    token);

                return HostBookingActionOutcome.Success;
            }, cancellationToken);
        }
    }
}

