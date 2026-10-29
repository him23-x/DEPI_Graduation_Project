using DiscoverEgypt.Application.Contracts.Storage;

namespace DiscoverEgypt.Application.Contracts.Storage;

public sealed record ExperienceEvidenceDownload(Stream Content, string ContentType);
