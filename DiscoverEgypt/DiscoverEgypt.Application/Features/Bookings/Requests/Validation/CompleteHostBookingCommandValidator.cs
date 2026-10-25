using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Validation;

public sealed class CompleteHostBookingCommandValidator : AbstractValidator<CompleteHostBookingCommand>
{
    public CompleteHostBookingCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.BookingId).GreaterThan(0);
    }
}
