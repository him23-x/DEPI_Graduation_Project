using DiscoverEgypt.Application.Features.AdminDisputes.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.AdminDisputes.Validators
{
    public sealed class GetAdminDisputeDetailsQueryValidator : AbstractValidator<GetAdminDisputeDetailsQuery>
    {
        public GetAdminDisputeDetailsQueryValidator() => RuleFor(query => query.DisputeId).GreaterThan(0);
    }
}
