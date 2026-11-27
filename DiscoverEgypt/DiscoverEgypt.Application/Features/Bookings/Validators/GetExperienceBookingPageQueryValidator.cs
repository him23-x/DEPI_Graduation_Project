using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators
{
    public sealed class GetExperienceBookingPageQueryValidator : AbstractValidator<GetExperienceBookingPageQuery>
    {
        public GetExperienceBookingPageQueryValidator() => RuleFor(query => query.ExperienceId).GreaterThan(0);
    }
}
