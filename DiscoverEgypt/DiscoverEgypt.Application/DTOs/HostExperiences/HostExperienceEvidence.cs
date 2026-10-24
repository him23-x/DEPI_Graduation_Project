using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public enum EvidenceExperienceType { Unsupported, Food, Arts, LocalTraditions }

public sealed record HostExperienceEvidenceFile(int Id, EvidenceKind Kind, DateTime UploadedAt);

public sealed record HostExperienceEvidenceEditor(
    int ExperienceId,
    string Title,
    string ExperienceTypeName,
    EvidenceExperienceType ExperienceType,
    string? TextContent,
    IReadOnlyList<HostExperienceEvidenceFile> Files);

public sealed record StoredExperienceEvidence(string StorageKey);

public sealed record ExperienceEvidenceDownload(Stream Content, string ContentType);

public enum ExperienceEvidenceMutationResult
{
    Success,
    ExperienceUnavailable,
    EvidenceUnavailable,
    InvalidEvidenceKind,
    InvalidFile,
    FileLimitReached,
    TextRequired,
    TextTooLong
}
