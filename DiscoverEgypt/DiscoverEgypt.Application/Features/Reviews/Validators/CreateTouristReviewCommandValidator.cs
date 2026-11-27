using DiscoverEgypt.Application.Features.Reviews.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Reviews.Validators
{
    public sealed class CreateTouristReviewCommandValidator : AbstractValidator<CreateTouristReviewCommand>
    {
        public CreateTouristReviewCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.Form).NotNull().DependentRules(() =>
            {
                RuleFor(command => command.Form.BookingId).GreaterThan(0);
                RuleFor(command => command.Form.Rating).InclusiveBetween(1, 5);
                RuleFor(command => command.Form.Comment).MaximumLength(2000);
            });
        }
    }
}
