using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record ExperienceMediaDownload(Stream Content, string ContentType);

