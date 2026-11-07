using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Contracts.Persistence
{
    public sealed class HostExperienceMediaMutationContext
    {
        public required Experience Experience { get; init; }
        public required Func<CancellationToken, Task> SaveChangesAsync { get; init; }
        public required Action<ExperienceMedia> RemoveMedia { get; init; }
    }
}
