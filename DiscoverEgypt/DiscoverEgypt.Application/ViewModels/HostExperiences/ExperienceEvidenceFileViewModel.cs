
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.ViewModels.HostExperiences
{
    public sealed record ExperienceEvidenceFileViewModel(int Id, EvidenceKind Kind, DateTime UploadedAt);
}

