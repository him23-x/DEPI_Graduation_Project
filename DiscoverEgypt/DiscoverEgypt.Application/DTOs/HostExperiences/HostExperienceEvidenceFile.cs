using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceEvidenceFile(int Id, EvidenceKind Kind, DateTime UploadedAt);

