using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators
{
    public sealed class GetTouristBookingsQueryValidator : AbstractValidator<GetTouristBookingsQuery>
    {
        public GetTouristBookingsQueryValidator() => RuleFor(query => query.UserId).NotEmpty();
    }
}
