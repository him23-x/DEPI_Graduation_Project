using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Validation;

public sealed class GetHostBookingDetailsQueryValidator : AbstractValidator<GetHostBookingDetailsQuery>
{
    public GetHostBookingDetailsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.BookingId).GreaterThan(0);
    }
}
