using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators
{
    public sealed class GetGuideBookingPageQueryValidator : AbstractValidator<GetGuideBookingPageQuery>
    {
        public GetGuideBookingPageQueryValidator() => RuleFor(query => query.GuideProfileId).GreaterThan(0);
    }
}
