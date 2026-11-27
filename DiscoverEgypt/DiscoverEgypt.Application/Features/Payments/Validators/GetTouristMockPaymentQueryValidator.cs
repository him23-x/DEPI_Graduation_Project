using DiscoverEgypt.Application.Features.Payments.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Payments.Validators
{
    public sealed class GetTouristMockPaymentQueryValidator : AbstractValidator<GetTouristMockPaymentQuery>
    {
        public GetTouristMockPaymentQueryValidator()
        {
            RuleFor(query => query.UserId).NotEmpty();
            RuleFor(query => query.BookingId).GreaterThan(0);
        }
    }
}
