using DiscoverEgypt.Application.Contracts.Storage;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Contracts.Storage;

public sealed record StoredExperienceMedia(string StorageKey, MediaType MediaType);
