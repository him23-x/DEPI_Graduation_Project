
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record EvidenceFileGroupDto(
    HostExperienceEvidenceEditorDto Editor,
    string Title,
    EvidenceKind Kind,
    int Minimum,
    int Maximum);

