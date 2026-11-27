using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.GuideVerification;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Commands;
using DiscoverEgypt.Application.Responses.GuideVerification;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminGuideVerification.Handlers.Commands
{
    public sealed class DecideAdminGuideVerificationCommandHandler
        : IRequestHandler<DecideAdminGuideVerificationCommand, AdminGuideVerificationDecisionResponse>
    {
        private readonly IAdminGuideVerificationRepository _repository;
        private readonly IValidator<DecideAdminGuideVerificationCommand> _validator;
        private readonly TimeProvider _timeProvider;

        public DecideAdminGuideVerificationCommandHandler(
            IAdminGuideVerificationRepository repository,
            IValidator<DecideAdminGuideVerificationCommand> validator,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _validator = validator;
            _timeProvider = timeProvider;
        }

        public async Task<AdminGuideVerificationDecisionResponse> Handle(
            DecideAdminGuideVerificationCommand request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var reason = request.Decision == GuideVerificationDecision.Reject
                ? request.Form.RejectionReason!.Trim()
                : null;
            var outcome = await _repository.DecideAsync(
                request.AdminUserId,
                request.VerificationRequestId,
                request.Decision,
                reason,
                _timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken);

            return new AdminGuideVerificationDecisionResponse(outcome, request.VerificationRequestId);
        }
    }
}
