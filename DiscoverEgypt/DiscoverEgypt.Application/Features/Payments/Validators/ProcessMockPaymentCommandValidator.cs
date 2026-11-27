using DiscoverEgypt.Application.Features.Payments.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Payments.Validators
{
    public sealed class ProcessMockPaymentCommandValidator : AbstractValidator<ProcessMockPaymentCommand>
    {
        public ProcessMockPaymentCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.BookingId).GreaterThan(0);
            RuleFor(command => command.Outcome).IsInEnum();
        }
    }
}
