using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Reviews;
using DiscoverEgypt.Domain.Enums.Bookings;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host.Reviews;

public sealed class HostReviewsRepository(DiscoverEgyptDbContext dbContext) : IHostReviewsRepository
{
    public async Task<HostReviewsPageDto> GetForHostAsync(
        string userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var hostReviews = dbContext.Reviews
            .AsNoTracking()
            .Where(review => review.Booking.ProviderUserId == userId
                && review.Booking.BookingType == BookingType.Experience
                && review.Booking.Experience != null
                && review.Booking.Experience.HostProfile.UserId == userId);

        var reviewCount = await hostReviews.CountAsync(cancellationToken);
        var averageRating = await hostReviews
            .AverageAsync(review => (decimal?)review.Rating, cancellationToken) ?? 0m;

        var items = await (
            from review in hostReviews
            join guest in dbContext.ApplicationUsers.AsNoTracking()
                on review.Booking.TouristUserId equals guest.Id
            orderby review.CreatedAt descending, review.Id descending
            select new
            {
                review.Id,
                review.BookingId,
                ExperienceTitle = review.Booking.Experience!.Title,
                guest.FullName,
                guest.UserName,
                review.Rating,
                review.Comment,
                review.CreatedAt
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var reviews = items.Select(item => new HostReviewListItemDto(
            item.Id,
            item.BookingId,
            item.ExperienceTitle,
            string.IsNullOrWhiteSpace(item.FullName) ? item.UserName ?? "Discover Egypt guest" : item.FullName,
            item.Rating,
            item.Comment,
            item.CreatedAt)).ToArray();

        return new HostReviewsPageDto(reviews, averageRating, reviewCount, page, pageSize, reviewCount);
    }
}
