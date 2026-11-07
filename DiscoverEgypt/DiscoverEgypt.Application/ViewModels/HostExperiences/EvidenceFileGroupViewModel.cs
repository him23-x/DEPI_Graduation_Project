
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.ViewModels.HostExperiences
{
    public sealed record EvidenceFileGroupViewModel(
        ExperienceEvidenceEditorViewModel Editor,
        string Title,
        EvidenceKind Kind,
        int Minimum,
        int Maximum);
}

