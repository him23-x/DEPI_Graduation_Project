using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Abstractions;

// The Application DEFINES the interface. The Infrastructure IMPLEMENTS it (Dependency Inversion).
public interface IExperienceRepository
{
    Task<IReadOnlyList<Experience>> GetPublishedAsync();
    Task<Experience?> GetByIdAsync(Guid id);
}
