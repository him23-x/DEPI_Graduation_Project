
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed record ExperienceEvidenceFileViewModel(int Id, EvidenceKind Kind, DateTime UploadedAt);
