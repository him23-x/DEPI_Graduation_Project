using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators
{
    public sealed class PreviewGuideBookingQueryValidator : AbstractValidator<PreviewGuideBookingQuery>
    {
        public PreviewGuideBookingQueryValidator()
        {
            RuleFor(query => query.Form).NotNull();
            When(query => query.Form is not null, () => GuideBookingRules.Validate(RuleFor(query => query.Form)));
        }
    }
}
