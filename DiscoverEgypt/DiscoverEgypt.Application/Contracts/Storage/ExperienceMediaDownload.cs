using DiscoverEgypt.Application.Contracts.Storage;

namespace DiscoverEgypt.Application.Contracts.Storage;

public sealed record ExperienceMediaDownload(Stream Content, string ContentType);
