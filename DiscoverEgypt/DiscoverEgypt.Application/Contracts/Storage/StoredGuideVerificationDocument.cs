using DiscoverEgypt.Domain.Enums.Verification;

namespace DiscoverEgypt.Application.Contracts.Storage
{
    public sealed record StoredGuideVerificationDocument(
        int DocumentId,
        VerificationDocType DocumentType,
        string StorageKey);
}
