using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminGuideVerification.Handlers.Queries
{
    public sealed class GetAdminGuideVerificationDocumentQueryHandler
        : IRequestHandler<GetAdminGuideVerificationDocumentQuery, ExperienceEvidenceDownload?>
    {
        private readonly IAdminGuideVerificationRepository _repository;
        private readonly IExperienceEvidenceStorage _storage;

        public GetAdminGuideVerificationDocumentQueryHandler(
            IAdminGuideVerificationRepository repository,
            IExperienceEvidenceStorage storage)
        {
            _repository = repository;
            _storage = storage;
        }

        public async Task<ExperienceEvidenceDownload?> Handle(
            GetAdminGuideVerificationDocumentQuery request,
            CancellationToken cancellationToken)
        {
            if (request.DocumentId <= 0) return null;
            var document = await _repository.GetDocumentForReviewAsync(request.DocumentId, cancellationToken);
            return document is null
                ? null
                : await _storage.OpenReadAsync(document.StorageKey, cancellationToken);
        }
    }
}
