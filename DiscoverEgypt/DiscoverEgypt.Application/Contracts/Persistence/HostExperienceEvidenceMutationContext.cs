using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public sealed class HostExperienceEvidenceMutationContext
    {
        public required Experience Experience { get; init; }
        public required Func<CancellationToken, Task> SaveChangesAsync { get; init; }
    }
}
