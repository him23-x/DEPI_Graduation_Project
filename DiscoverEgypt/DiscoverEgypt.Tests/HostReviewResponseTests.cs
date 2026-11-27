using AutoMapper;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Reviews.Handlers.Queries;
using DiscoverEgypt.Application.Features.Reviews.Requests.Queries;
using DiscoverEgypt.Application.Features.Reviews.Validators;
using DiscoverEgypt.Application.Mapping;
using DiscoverEgypt.Application.Responses.Accounts;
using DiscoverEgypt.Application.Responses.Reviews;
using DiscoverEgypt.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DiscoverEgypt.Tests;

public sealed class HostReviewResponseTests
{
    [Fact]
    public async Task Handler_ProjectsNumericReviewsAndAggregatesForRequestedHost()
    {
        var createdAt = new DateTime(2026, 7, 18, 9, 30, 0, DateTimeKind.Utc);
        var review = new Review
        {
            Id = 75,
            BookingId = 240,
            Rating = 5,
            Comment = "A wonderful cultural experience.",
            CreatedAt = createdAt,
            Booking = new Booking
            {
                TouristUserId = "tourist-8",
                Experience = new Experience { Title = "Coptic Cairo Walk" }
            }
        };
        var repository = new FakeHostReviewsRepository([review]);
        using var loggerFactory = LoggerFactory.Create(_ => { });
        var mapper = new MapperConfiguration(
            configuration => configuration.AddProfile<HostReviewsMappingProfile>(),
            loggerFactory).CreateMapper();
        var handler = new GetHostReviewsQueryHandler(
            repository,
            mapper,
            new GetHostReviewsQueryValidator());

        var response = await handler.Handle(new GetHostReviewsQuery("host-234", Page: 2), CancellationToken.None);

        Assert.Equal("host-234", repository.ReviewHostId);
        Assert.Equal("host-234", repository.CountHostId);
        Assert.Equal("host-234", repository.AverageHostId);
        Assert.Equal(2, repository.RequestedPage);
        Assert.Equal(10, repository.RequestedPageSize);
        Assert.Equal("tourist-8", Assert.Single(repository.RequestedTouristIds));
        Assert.Equal(4.5m, response.AverageRating);
        Assert.Equal(12, response.ReviewCount);
        Assert.Equal(12, response.TotalCount);
        Assert.Equal(2, response.Page);
        Assert.Equal(2, response.PageCount);

        var item = Assert.Single(response.Reviews);
        Assert.Equal(75, item.ReviewId);
        Assert.Equal(240, item.BookingId);
        Assert.Equal("Coptic Cairo Walk", item.ExperienceTitle);
        Assert.Equal("Guest Name", item.GuestName);
        Assert.Equal(5, item.Rating);
        Assert.Equal("A wonderful cultural experience.", item.Comment);
        Assert.Equal(createdAt, item.CreatedAt);
    }

    [Fact]
    public async Task Handler_UsesGuestFallbackWhenTouristNameIsUnavailable()
    {
        var review = new Review
        {
            Id = 76,
            BookingId = 241,
            Rating = 4,
            CreatedAt = DateTime.UtcNow,
            Booking = new Booking
            {
                TouristUserId = "tourist-9",
                Experience = new Experience { Title = "Pottery Class" }
            }
        };
        var repository = new FakeHostReviewsRepository([review], includeTourist: false);
        using var loggerFactory = LoggerFactory.Create(_ => { });
        var mapper = new MapperConfiguration(
            configuration => configuration.AddProfile<HostReviewsMappingProfile>(),
            loggerFactory).CreateMapper();
        var handler = new GetHostReviewsQueryHandler(
            repository,
            mapper,
            new GetHostReviewsQueryValidator());

        var response = await handler.Handle(new GetHostReviewsQuery("host-234"), CancellationToken.None);

        Assert.Equal("Discover Egypt guest", Assert.Single(response.Reviews).GuestName);
    }

    private sealed class FakeHostReviewsRepository(
        IReadOnlyList<Review> reviews,
        bool includeTourist = true) : IHostReviewsRepository
    {
        public string? ReviewHostId { get; private set; }
        public string? CountHostId { get; private set; }
        public string? AverageHostId { get; private set; }
        public int RequestedPage { get; private set; }
        public int RequestedPageSize { get; private set; }
        public List<string> RequestedTouristIds { get; } = [];

        public Task<IReadOnlyList<Review>> GetForHostAsync(
            string userId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            ReviewHostId = userId;
            RequestedPage = page;
            RequestedPageSize = pageSize;
            return Task.FromResult(reviews);
        }

        public Task<int> GetCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            CountHostId = userId;
            return Task.FromResult(12);
        }

        public Task<decimal> GetAverageRatingAsync(string userId, CancellationToken cancellationToken = default)
        {
            AverageHostId = userId;
            return Task.FromResult(4.5m);
        }

        public Task<IReadOnlyDictionary<string, UserSummaryResponse>> GetTouristsAsync(
            IEnumerable<string> userIds,
            CancellationToken cancellationToken = default)
        {
            RequestedTouristIds.AddRange(userIds);
            IReadOnlyDictionary<string, UserSummaryResponse> tourists = includeTourist
                ? new Dictionary<string, UserSummaryResponse>
                {
                    ["tourist-8"] = new("tourist-8", "guest-user", "guest@example.com", "Guest Name")
                }
                : new Dictionary<string, UserSummaryResponse>();
            return Task.FromResult(tourists);
        }
    }
}
