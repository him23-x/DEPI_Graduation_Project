
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed record ExperienceEvidenceSummaryViewModel(EvidenceKind Kind, string? TextContent, int FileCount);
