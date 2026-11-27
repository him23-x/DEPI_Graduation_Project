using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators
{
    public sealed class GetTouristGuideBookingConfirmationQueryValidator : AbstractValidator<GetTouristGuideBookingConfirmationQuery>
    {
        public GetTouristGuideBookingConfirmationQueryValidator()
        {
            RuleFor(query => query.UserId).NotEmpty();
            RuleFor(query => query.BookingId).GreaterThan(0);
        }
    }
}
