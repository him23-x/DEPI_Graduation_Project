using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators
{
    public sealed class CancelTouristBookingCommandValidator : AbstractValidator<CancelTouristBookingCommand>
    {
        public CancelTouristBookingCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.BookingId).GreaterThan(0);
        }
    }
}
