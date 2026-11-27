using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Queries;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Validators;
using DiscoverEgypt.Application.Responses.GuideVerification;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminGuideVerification.Handlers.Queries
{
    public sealed class GetAdminGuideVerificationDetailsQueryHandler
        : IRequestHandler<GetAdminGuideVerificationDetailsQuery, AdminGuideVerificationDetailsResponse?>
    {
        private readonly IAdminGuideVerificationRepository _repository;
        private readonly IValidator<GetAdminGuideVerificationDetailsQuery> _validator;

        public GetAdminGuideVerificationDetailsQueryHandler(
            IAdminGuideVerificationRepository repository,
            IValidator<GetAdminGuideVerificationDetailsQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<AdminGuideVerificationDetailsResponse?> Handle(
            GetAdminGuideVerificationDetailsQuery request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _repository.GetPendingDetailsAsync(request.VerificationRequestId, cancellationToken);
        }
    }
}
