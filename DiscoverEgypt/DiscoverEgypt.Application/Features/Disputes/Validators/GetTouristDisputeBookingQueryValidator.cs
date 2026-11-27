using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Disputes.Validators
{
    public sealed class GetTouristDisputeBookingQueryValidator : AbstractValidator<GetTouristDisputeBookingQuery>
    {
        public GetTouristDisputeBookingQueryValidator()
        {
            RuleFor(query => query.UserId).NotEmpty();
            RuleFor(query => query.BookingId).GreaterThan(0);
        }
    }
}
