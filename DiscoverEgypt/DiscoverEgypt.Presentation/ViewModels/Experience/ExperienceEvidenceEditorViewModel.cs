using DiscoverEgypt.Application.DTOs.HostExperiences;

using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed record ExperienceEvidenceEditorViewModel(
    int ExperienceId,
    string Title,
    string ExperienceTypeName,
    EvidenceExperienceType ExperienceType,
    string? TextContent,
    IReadOnlyList<ExperienceEvidenceFileViewModel> Files);
