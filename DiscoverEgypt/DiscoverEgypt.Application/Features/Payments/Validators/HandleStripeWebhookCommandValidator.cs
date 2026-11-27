using DiscoverEgypt.Application.Features.Payments.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Payments.Validators
{
    public sealed class HandleStripeWebhookCommandValidator : AbstractValidator<HandleStripeWebhookCommand>
    {
        public HandleStripeWebhookCommandValidator()
        {
            RuleFor(command => command.Payload).NotEmpty();
            RuleFor(command => command.SignatureHeader).NotEmpty();
        }
    }
}
