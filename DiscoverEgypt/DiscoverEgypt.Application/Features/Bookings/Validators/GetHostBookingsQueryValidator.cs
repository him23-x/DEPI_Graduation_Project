using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators;

public sealed class GetHostBookingsQueryValidator : AbstractValidator<GetHostBookingsQuery>
{
    public GetHostBookingsQueryValidator() => RuleFor(x => x.UserId).NotEmpty();
}
