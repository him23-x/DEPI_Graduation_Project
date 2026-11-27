using DiscoverEgypt.Application.Features.Explore.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Explore.Validators
{
    public sealed class ExploreExperiencesQueryValidator : AbstractValidator<ExploreExperiencesQuery>
    {
        public ExploreExperiencesQueryValidator()
        {
            RuleFor(query => query.Search).MaximumLength(150);
            RuleFor(query => query.Page).GreaterThan(0);
            RuleFor(query => query.PageSize).InclusiveBetween(1, 24);
            RuleFor(query => query.TypeId).GreaterThan(0).When(query => query.TypeId.HasValue);
            RuleFor(query => query.AreaId).GreaterThan(0).When(query => query.AreaId.HasValue);
            RuleFor(query => query.MinPrice).GreaterThanOrEqualTo(0).When(query => query.MinPrice.HasValue);
            RuleFor(query => query.MaxPrice).GreaterThanOrEqualTo(0).When(query => query.MaxPrice.HasValue);
            RuleFor(query => query.Sort)
                .Must(sort => string.IsNullOrWhiteSpace(sort)
                    || sort is "newest" or "price-low" or "price-high" or "rating")
                .WithMessage("Choose a supported sort order.");
        }
    }
}
