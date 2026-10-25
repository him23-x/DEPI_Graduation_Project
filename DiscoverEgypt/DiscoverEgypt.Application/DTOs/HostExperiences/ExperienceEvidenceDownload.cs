using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record ExperienceEvidenceDownload(Stream Content, string ContentType);

