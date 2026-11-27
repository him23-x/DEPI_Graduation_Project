using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Disputes;
using DiscoverEgypt.Application.Features.AdminDisputes.Requests.Commands;
using DiscoverEgypt.Application.Responses.Disputes;
using DiscoverEgypt.Domain.Enums.Disputes;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminDisputes.Handlers.Commands
{
    public sealed class ResolveAdminDisputeCommandHandler
        : IRequestHandler<ResolveAdminDisputeCommand, AdminDisputeResolutionResponse>
    {
        private readonly IAdminDisputeReviewRepository _repository;
        private readonly IValidator<ResolveAdminDisputeCommand> _validator;
        private readonly TimeProvider _timeProvider;

        public ResolveAdminDisputeCommandHandler(
            IAdminDisputeReviewRepository repository,
            IValidator<ResolveAdminDisputeCommand> validator,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _validator = validator;
            _timeProvider = timeProvider;
        }

        public async Task<AdminDisputeResolutionResponse> Handle(
            ResolveAdminDisputeCommand request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            var outcome = request.Form.Outcome!.Value;
            int? sanctionDays = outcome switch
            {
                DisputeOutcome.ProviderFault => request.Form.SanctionDays ?? 14,
                DisputeOutcome.TouristFault => request.Form.SanctionDays ?? 30,
                DisputeOutcome.NoFault => null,
                _ => null
            };
            var note = string.IsNullOrWhiteSpace(request.Form.ResolutionNote)
                ? null
                : request.Form.ResolutionNote.Trim();
            var result = await _repository.ResolveAsync(
                request.AdminUserId,
                request.DisputeId,
                outcome,
                note,
                sanctionDays,
                _timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken);

            return new AdminDisputeResolutionResponse(result, request.DisputeId);
        }
    }
}
