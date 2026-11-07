
using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.ViewModels.HostExperiences
{
    public sealed record ExperienceEvidenceEditorViewModel(
        int ExperienceId,
        string Title,
        string ExperienceTypeName,
        EvidenceExperienceType ExperienceType,
        string? TextContent,
        IReadOnlyList<ExperienceEvidenceFileViewModel> Files);
}

