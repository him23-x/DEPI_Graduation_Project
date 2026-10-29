using DiscoverEgypt.Application.Features.Dashboard.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Dashboard.Validators;

public sealed class GetHostDashboardQueryValidator : AbstractValidator<GetHostDashboardQuery>
{
    public GetHostDashboardQueryValidator() => RuleFor(query => query.UserId).NotEmpty();
}
