using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed record EvidenceFileGroupViewModel(
    HostExperienceEvidenceEditor Editor,
    string Title,
    EvidenceKind Kind,
    int Minimum,
    int Maximum);
