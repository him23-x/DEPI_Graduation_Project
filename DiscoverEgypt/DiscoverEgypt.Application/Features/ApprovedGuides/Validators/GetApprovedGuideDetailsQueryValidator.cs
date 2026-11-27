using DiscoverEgypt.Application.Features.ApprovedGuides.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.ApprovedGuides.Validators
{
    public sealed class GetApprovedGuideDetailsQueryValidator : AbstractValidator<GetApprovedGuideDetailsQuery>
    {
        public GetApprovedGuideDetailsQueryValidator() => RuleFor(query => query.GuideProfileId).GreaterThan(0);
    }
}
