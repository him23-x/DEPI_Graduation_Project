using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Validation;

public sealed class AcceptHostBookingCommandValidator : AbstractValidator<AcceptHostBookingCommand>
{
    public AcceptHostBookingCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.BookingId).GreaterThan(0);
    }
}
