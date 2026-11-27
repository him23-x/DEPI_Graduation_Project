using DiscoverEgypt.Application.Features.ApprovedGuides.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.ApprovedGuides.Validators
{
    public sealed class GetApprovedGuidesQueryValidator : AbstractValidator<GetApprovedGuidesQuery>
    {
        public GetApprovedGuidesQueryValidator()
        {
            RuleFor(query => query.Page).GreaterThan(0);
            RuleFor(query => query.PageSize).InclusiveBetween(1, 24);
            RuleFor(query => query.LanguageId).GreaterThan(0).When(query => query.LanguageId.HasValue);
            RuleFor(query => query.CulturalAreaId).GreaterThan(0).When(query => query.CulturalAreaId.HasValue);
            RuleFor(query => query.Search).MaximumLength(120);
        }
    }
}
