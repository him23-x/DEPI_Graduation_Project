
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.ViewModels.HostExperiences
{
    public sealed record ExperienceEvidenceSummaryViewModel(EvidenceKind Kind, string? TextContent, int FileCount);
}

