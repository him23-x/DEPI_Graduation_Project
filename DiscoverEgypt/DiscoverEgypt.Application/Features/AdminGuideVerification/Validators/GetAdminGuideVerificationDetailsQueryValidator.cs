using DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Queries;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.AdminGuideVerification.Validators
{
    public sealed class GetAdminGuideVerificationDetailsQueryValidator
        : AbstractValidator<GetAdminGuideVerificationDetailsQuery>
    {
        public GetAdminGuideVerificationDetailsQueryValidator() =>
            RuleFor(query => query.VerificationRequestId).GreaterThan(0);
    }
}
