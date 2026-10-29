using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Enums.HostExperiences;

public enum ExperienceEvidenceMutationOutcome
{
    Success, ExperienceUnavailable, EvidenceUnavailable, InvalidEvidenceKind,
    InvalidFile, FileLimitReached, TextRequired, TextTooLong
}
