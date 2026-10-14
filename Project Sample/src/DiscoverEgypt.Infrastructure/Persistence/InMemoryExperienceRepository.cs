using DiscoverEgypt.Application.Abstractions;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Infrastructure.Persistence;

// In-memory implementation so the sample runs with NO database.
// Later you replace it with an Entity Framework Core implementation of the SAME interface.
public class InMemoryExperienceRepository : IExperienceRepository
{
    private static readonly Guid HostId = Guid.Parse("00000000-0000-0000-0000-0000000000a1");

    private readonly List<Experience> _items = new()
    {
        new Experience(Guid.Parse("11111111-1111-1111-1111-111111111111"), HostId,
            "طبخ مصري تقليدي مع عائلة",
            "تتعلم تحضّر أكلات مصرية في بيت عائلة، وتاكل معاهم.",
            800m, 180, 6, "الجيزة", ExperienceStatus.Published),

        new Experience(Guid.Parse("22222222-2222-2222-2222-222222222222"), HostId,
            "ورشة فخار مع حرفي",
            "تجرّب صناعة الفخار بإيدك مع حرفي متخصص.",
            450m, 120, 8, "مصر القديمة", ExperienceStatus.Published),

        new Experience(Guid.Parse("33333333-3333-3333-3333-333333333333"), HostId,
            "يوم في مزرعة",
            "يوم كامل في مزرعة مع فلاح: زراعة وحصاد وغدا.",
            600m, 360, 10, "الفيوم", ExperienceStatus.Published),

        // Draft: it must NOT appear in the public list.
        new Experience(Guid.Parse("44444444-4444-4444-4444-444444444444"), HostId,
            "تجربة قيد الإعداد",
            "لسه Draft.",
            300m, 90, 4, "القاهرة", ExperienceStatus.Draft),
    };

    public Task<IReadOnlyList<Experience>> GetPublishedAsync() =>
        Task.FromResult<IReadOnlyList<Experience>>(
            _items.Where(e => e.Status == ExperienceStatus.Published).ToList());

    public Task<Experience?> GetByIdAsync(Guid id) =>
        Task.FromResult(_items.FirstOrDefault(e => e.Id == id));
}
