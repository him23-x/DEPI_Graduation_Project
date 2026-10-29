
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceEvidenceFileDto(int Id, EvidenceKind Kind, DateTime UploadedAt);
