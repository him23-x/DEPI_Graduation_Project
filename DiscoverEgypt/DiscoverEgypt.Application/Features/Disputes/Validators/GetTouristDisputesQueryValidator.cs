using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Disputes.Validators
{
    public sealed class GetTouristDisputesQueryValidator : AbstractValidator<GetTouristDisputesQuery>
    {
        public GetTouristDisputesQueryValidator() => RuleFor(query => query.UserId).NotEmpty();
    }
}
