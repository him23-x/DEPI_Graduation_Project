using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Bookings.Policies;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using DiscoverEgypt.Domain.Enums.Bookings;
using MediatR;
using DiscoverEgypt.Application.Enums.Bookings;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Commands
{
    public sealed class CompleteHostBookingCommandHandler : IRequestHandler<CompleteHostBookingCommand, HostBookingActionOutcome>
    {
        private readonly IHostBookingRepository _repository;
        private readonly IValidator<CompleteHostBookingCommand> _validator;

        public CompleteHostBookingCommandHandler(IHostBookingRepository repository, IValidator<CompleteHostBookingCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<HostBookingActionOutcome> Handle(CompleteHostBookingCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            return await _repository.ExecuteActionAsync(request.UserId, request.BookingId, (context, _) =>
            {
                var booking = context.Booking;
                if (booking is null)
                {
                    return Task.FromResult(HostBookingActionOutcome.BookingUnavailable);
                }

                var failure = HostBookingPolicy.GetCompleteFailure(
                    booking.Status,
                    booking.ScheduledEnd,
                    context.HasOpenDispute,
                    context.Now);
                if (failure.HasValue)
                {
                    return Task.FromResult(failure.Value);
                }

                booking.Status = BookingStatus.Completed;
                return Task.FromResult(HostBookingActionOutcome.Success);
            }, cancellationToken);
        }
    }
}

