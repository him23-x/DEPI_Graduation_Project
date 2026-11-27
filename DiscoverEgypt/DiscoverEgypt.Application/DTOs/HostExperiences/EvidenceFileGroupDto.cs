
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences
{
    public sealed record EvidenceFileGroupDto(
        ExperienceEvidenceEditorDto Editor,
        string Title,
        EvidenceKind Kind,
        int Minimum,
        int Maximum);
}

