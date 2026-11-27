using DiscoverEgypt.Application.Contracts.Storage;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Queries
{
    public sealed record GetAdminGuideVerificationDocumentQuery(int DocumentId)
        : IRequest<ExperienceEvidenceDownload?>;
}
