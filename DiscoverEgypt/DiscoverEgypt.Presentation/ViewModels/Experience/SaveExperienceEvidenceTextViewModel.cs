
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed class SaveExperienceEvidenceTextViewModel
{
    public int ExperienceId { get; set; }
    public EvidenceKind Kind { get; set; }
    public string? TextContent { get; set; }
}
