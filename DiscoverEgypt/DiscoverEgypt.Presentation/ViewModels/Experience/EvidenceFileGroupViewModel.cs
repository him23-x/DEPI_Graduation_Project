
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed record EvidenceFileGroupViewModel(
    ExperienceEvidenceEditorViewModel Editor,
    string Title,
    EvidenceKind Kind,
    int Minimum,
    int Maximum);
