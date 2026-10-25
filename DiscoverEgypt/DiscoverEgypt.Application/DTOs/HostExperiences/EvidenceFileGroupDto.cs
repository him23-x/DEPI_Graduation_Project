using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record EvidenceFileGroupDto(
    HostExperienceEvidenceEditor Editor,
    string Title,
    EvidenceKind Kind,
    int Minimum,
    int Maximum);

