using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators
{
    public sealed class CreateGuideBookingCommandValidator : AbstractValidator<CreateGuideBookingCommand>
    {
        public CreateGuideBookingCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.Form).NotNull();
            When(command => command.Form is not null, () => GuideBookingRules.Validate(RuleFor(command => command.Form)));
        }
    }
}
