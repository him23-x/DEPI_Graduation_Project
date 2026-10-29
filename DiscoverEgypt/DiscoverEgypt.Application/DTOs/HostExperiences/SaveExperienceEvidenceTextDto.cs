
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed class SaveExperienceEvidenceTextDto
{
    public int ExperienceId { get; set; }
    public EvidenceKind Kind { get; set; }
    public string? TextContent { get; set; }
}

