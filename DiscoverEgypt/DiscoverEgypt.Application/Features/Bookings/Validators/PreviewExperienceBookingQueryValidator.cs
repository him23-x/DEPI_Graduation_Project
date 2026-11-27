using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators
{
    public sealed class PreviewExperienceBookingQueryValidator : AbstractValidator<PreviewExperienceBookingQuery>
    {
        public PreviewExperienceBookingQueryValidator()
        {
            RuleFor(query => query.Form).NotNull();
            When(query => query.Form is not null, () =>
            {
                RuleFor(query => query.Form.ExperienceId).GreaterThan(0);
                RuleFor(query => query.Form.NumberOfGuests).NotNull().GreaterThan(0);
                RuleFor(query => query.Form.ScheduledStartLocal)
                    .NotNull()
                    .Must(value => value is not null && value.Value != default)
                    .WithMessage("Choose a start date and time.");
            });
        }
    }
}
