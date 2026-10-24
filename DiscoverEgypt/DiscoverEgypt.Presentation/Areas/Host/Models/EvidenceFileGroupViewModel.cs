using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Presentation.Areas.Host.Models;

public sealed record EvidenceFileGroupViewModel(
    HostExperienceEvidenceEditor Editor,
    string Title,
    EvidenceKind Kind,
    int Minimum,
    int Maximum);
