using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Abstractions;

public interface IBookingRepository
{
    Task AddAsync(Booking booking);
    Task<IReadOnlyList<Booking>> GetByTouristAsync(Guid touristId);
}
