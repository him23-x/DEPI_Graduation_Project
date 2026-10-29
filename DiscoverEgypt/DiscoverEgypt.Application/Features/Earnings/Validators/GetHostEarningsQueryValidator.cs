using DiscoverEgypt.Application.Features.Earnings.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Earnings.Validators;

public sealed class GetHostEarningsQueryValidator : AbstractValidator<GetHostEarningsQuery>
{
    public GetHostEarningsQueryValidator()
    {
        RuleFor(query => query.UserId).NotEmpty();
        RuleFor(query => query.Status).IsInEnum().When(query => query.Status.HasValue);
        RuleFor(query => query.Page).InclusiveBetween(1, 1_000_000);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 50);
    }
}
