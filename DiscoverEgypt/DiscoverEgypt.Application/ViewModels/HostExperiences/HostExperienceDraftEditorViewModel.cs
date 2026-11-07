
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.ViewModels.HostExperiences
{
    public sealed record HostExperienceDraftEditorViewModel(
        int ExperienceId,
        ExperienceStatus Status,
        string? RejectionReason,
        HostExperienceDraftViewModel Draft);
}

