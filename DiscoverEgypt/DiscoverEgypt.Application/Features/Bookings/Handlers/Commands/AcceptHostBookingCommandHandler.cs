using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Notifications;
using DiscoverEgypt.Application.Features.Bookings.Options;
using DiscoverEgypt.Application.Features.Bookings.Policies;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Notifications;
using DiscoverEgypt.Domain.Enums.Payments;
using MediatR;
using DiscoverEgypt.Application.Enums.Bookings;
using Microsoft.Extensions.Options;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands
{
    public sealed class AcceptHostBookingCommandHandler : IRequestHandler<AcceptHostBookingCommand, HostBookingActionOutcome>
    {
        private readonly IHostBookingRepository _repository;
        private readonly INotificationService _notificationService;
        private readonly IOptions<BookingLifecycleOptions> _lifecycleOptions;
        private readonly IValidator<AcceptHostBookingCommand> _validator;

        public AcceptHostBookingCommandHandler(
            IHostBookingRepository repository,
            INotificationService notificationService,
            IOptions<BookingLifecycleOptions> lifecycleOptions,
            IValidator<AcceptHostBookingCommand> validator)
        {
            _repository = repository;
            _notificationService = notificationService;
            _lifecycleOptions = lifecycleOptions;
            _validator = validator;
        }

        public async Task<HostBookingActionOutcome> Handle(AcceptHostBookingCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            return await _repository.ExecuteActionAsync(request.UserId, request.BookingId, async (context, token) =>
            {
                var booking = context.Booking;
                if (booking is null) return HostBookingActionOutcome.BookingUnavailable;

                var failure = HostBookingPolicy.GetAcceptFailure(
                    booking.Status,
                    booking.Payment.Status,
                    booking.Payment.PaidAt,
                    context.IsProviderSuspended,
                    context.HasOverlap,
                    context.HasPayout,
                    context.Now,
                    _lifecycleOptions.Value.ProviderResponseTimeout);
                if (failure.HasValue) return failure.Value;

                booking.Status = BookingStatus.Confirmed;
                context.AddPayout(new Payout
                {
                    BookingId = booking.Id,
                    Amount = booking.ServiceAmount,
                    EligibleAt = booking.ScheduledEnd.AddHours(24),
                    Status = PayoutStatus.Pending
                });
                await _notificationService.CreateAsync(
                    booking.TouristUserId,
                    NotificationType.BookingConfirmation,
                    $"Your booking for {booking.Experience!.Title} has been confirmed.",
                    booking.Id,
                    token);

                return HostBookingActionOutcome.Success;
            }, cancellationToken);
        }
    }
}

