using DiscoverEgypt.Application.Features.Reviews.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Reviews.Validators;

public sealed class GetHostReviewsQueryValidator : AbstractValidator<GetHostReviewsQuery>
{
    public GetHostReviewsQueryValidator()
    {
        RuleFor(query => query.UserId).NotEmpty();
        RuleFor(query => query.Page).InclusiveBetween(1, 1_000_000);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 50);
    }
}
