using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Disputes.Validators
{
    public sealed class GetTouristDisputeDetailsQueryValidator : AbstractValidator<GetTouristDisputeDetailsQuery>
    {
        public GetTouristDisputeDetailsQueryValidator()
        {
            RuleFor(query => query.UserId).NotEmpty();
            RuleFor(query => query.DisputeId).GreaterThan(0);
        }
    }
}
