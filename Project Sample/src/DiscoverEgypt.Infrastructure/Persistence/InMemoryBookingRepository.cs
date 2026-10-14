using DiscoverEgypt.Application.Abstractions;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Infrastructure.Persistence;

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly List<Booking> _items = new();
    private readonly object _lock = new();

    public Task AddAsync(Booking booking)
    {
        lock (_lock) { _items.Add(booking); }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Booking>> GetByTouristAsync(Guid touristId)
    {
        lock (_lock)
        {
            IReadOnlyList<Booking> result = _items
                .Where(b => b.TouristId == touristId)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();
            return Task.FromResult(result);
        }
    }
}
