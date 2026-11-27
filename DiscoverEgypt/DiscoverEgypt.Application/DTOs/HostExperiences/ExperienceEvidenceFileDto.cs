
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences
{
    public sealed record ExperienceEvidenceFileDto(int Id, EvidenceKind Kind, DateTime UploadedAt);
}

