using DiscoverEgypt.Application.Features.BookingShares.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.BookingShares.Validators
{
    public sealed class CreateTouristBookingShareCommandValidator : AbstractValidator<CreateTouristBookingShareCommand>
    {
        public CreateTouristBookingShareCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.BookingId).GreaterThan(0);
        }
    }
}
