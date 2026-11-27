using DiscoverEgypt.Application.Features.Recommendations.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Recommendations.Validators
{
    public sealed class GetRecommendedExperiencesQueryValidator : AbstractValidator<GetRecommendedExperiencesQuery>
    {
        public GetRecommendedExperiencesQueryValidator()
        {
            RuleFor(query => query.UserId).NotEmpty();
        }
    }
}
