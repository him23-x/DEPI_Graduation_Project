using DiscoverEgypt.Application.Features.Payments.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Payments.Validators
{
    public sealed class CreateStripeCheckoutSessionCommandValidator : AbstractValidator<CreateStripeCheckoutSessionCommand>
    {
        public CreateStripeCheckoutSessionCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.BookingId).GreaterThan(0);
            RuleFor(command => command.SuccessUrl).NotEmpty();
            RuleFor(command => command.CancelUrl).NotEmpty();
        }
    }
}
