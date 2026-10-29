using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Disputes.Validators;

public sealed class GetHostDisputesQueryValidator : AbstractValidator<GetHostDisputesQuery>
{
    public GetHostDisputesQueryValidator()
    {
        RuleFor(query => query.UserId).NotEmpty();
        RuleFor(query => query.Page).InclusiveBetween(1, 1_000_000);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 50);
    }
}
