using DiscoverEgypt.Domain.Enums.Verification;

namespace DiscoverEgypt.Application.Responses.GuideVerification
{
    public sealed record AdminGuideVerificationDocumentResponse(
        int DocumentId,
        VerificationDocType DocumentType,
        string? Language,
        string? CulturalArea,
        DateTime UploadedAt);
}
