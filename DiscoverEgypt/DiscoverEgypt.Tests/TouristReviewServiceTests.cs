using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Reviews;
using DiscoverEgypt.Application.Enums.Reviews;
using DiscoverEgypt.Application.Features.Reviews.Policies;
using DiscoverEgypt.Application.Features.Reviews.Requests.Commands;
using DiscoverEgypt.Application.Features.Reviews.Services;
using DiscoverEgypt.Application.Features.Reviews.Validators;
using DiscoverEgypt.Application.Responses.Reviews;
using DiscoverEgypt.Domain.Enums.Bookings;
using FluentValidation;

namespace DiscoverEgypt.Tests
{
    public sealed class TouristReviewServiceTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);

        [Fact]
        public async Task CreateCompletedBooking_StoresRatingAndTrimmedCommentAtUtc()
        {
            var repository = new FakeTouristReviewRepository(BookingStatus.Completed, hasReview: false);
            var response = await CreateService(repository).CreateAsync(
                "tourist-1", 31, 5, "  Wonderful visit.  ", CancellationToken.None);

            Assert.Equal(TouristReviewCreateOutcome.Created, response.Outcome);
            Assert.Equal(5, repository.CreatedRating);
            Assert.Equal("Wonderful visit.", repository.CreatedComment);
            Assert.Equal(DateTimeKind.Utc, repository.CreatedAtUtc?.Kind);
        }

        [Fact]
        public async Task Create_RejectsAnotherTouristsBooking()
        {
            var repository = new FakeTouristReviewRepository(BookingStatus.Completed, hasReview: false)
            {
                OwnerUserId = "someone-else"
            };

            var response = await CreateService(repository).CreateAsync(
                "tourist-1", 31, 4, null, CancellationToken.None);

            Assert.Equal(TouristReviewCreateOutcome.BookingUnavailable, response.Outcome);
        }

        [Theory]
        [InlineData(BookingStatus.Pending)]
        [InlineData(BookingStatus.Confirmed)]
        [InlineData(BookingStatus.Cancelled)]
        public async Task Create_RejectsNonCompletedBooking(BookingStatus status)
        {
            var repository = new FakeTouristReviewRepository(status, hasReview: false);

            var response = await CreateService(repository).CreateAsync(
                "tourist-1", 31, 4, null, CancellationToken.None);

            Assert.Equal(TouristReviewCreateOutcome.BookingNotCompleted, response.Outcome);
        }

        [Fact]
        public async Task Create_RejectsSecondReview()
        {
            var repository = new FakeTouristReviewRepository(BookingStatus.Completed, hasReview: true);

            var response = await CreateService(repository).CreateAsync(
                "tourist-1", 31, 4, null, CancellationToken.None);

            Assert.Equal(TouristReviewCreateOutcome.AlreadyReviewed, response.Outcome);
        }

        [Fact]
        public async Task Create_AllowsMissingOptionalComment()
        {
            var repository = new FakeTouristReviewRepository(BookingStatus.Completed, hasReview: false);

            var response = await CreateService(repository).CreateAsync(
                "tourist-1", 31, 3, "   ", CancellationToken.None);

            Assert.Equal(TouristReviewCreateOutcome.Created, response.Outcome);
            Assert.Null(repository.CreatedComment);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public async Task Validator_AcceptsRatingBoundaries(int rating)
        {
            var validator = new CreateTouristReviewCommandValidator();
            var result = await validator.ValidateAsync(new CreateTouristReviewCommand(
                "tourist-1", new CreateTouristReviewDto { BookingId = 31, Rating = rating }));

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async Task Validator_RejectsRatingsOutsideOneThroughFive(int rating)
        {
            var validator = new CreateTouristReviewCommandValidator();
            var result = await validator.ValidateAsync(new CreateTouristReviewCommand(
                "tourist-1", new CreateTouristReviewDto { BookingId = 31, Rating = rating }));

            Assert.False(result.IsValid);
        }

        [Fact]
        public void BookingDetailsExposeNumericRatingForTextPresentation()
        {
            Assert.Equal(typeof(int?), typeof(DiscoverEgypt.Application.Responses.Bookings.TouristBookingDetailsResponse)
                .GetProperty("ReviewRating")?.PropertyType);
            Assert.False(TouristReviewEligibilityPolicy.CanReview(BookingStatus.Completed, hasReview: true));
        }

        private static TouristReviewService CreateService(FakeTouristReviewRepository repository) =>
            new(repository, new FixedTimeProvider(Now));

        private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
        }

        private sealed class FakeTouristReviewRepository(BookingStatus status, bool hasReview) : ITouristReviewRepository
        {
            public string OwnerUserId { get; init; } = "tourist-1";
            public int? CreatedRating { get; private set; }
            public string? CreatedComment { get; private set; }
            public DateTime? CreatedAtUtc { get; private set; }

            public Task<TouristReviewBookingResponse?> GetOwnedBookingAsync(
                string userId,
                int bookingId,
                CancellationToken cancellationToken = default) =>
                Task.FromResult<TouristReviewBookingResponse?>(userId == OwnerUserId && bookingId == 31
                    ? new TouristReviewBookingResponse(31, "Cairo Food Walk", status, hasReview)
                    : null);

            public Task<TouristReviewCreateResponse> CreateAsync(
                string userId,
                int bookingId,
                int rating,
                string? comment,
                DateTime nowUtc,
                CancellationToken cancellationToken = default)
            {
                CreatedRating = rating;
                CreatedComment = comment;
                CreatedAtUtc = nowUtc;
                return Task.FromResult(new TouristReviewCreateResponse(TouristReviewCreateOutcome.Created, bookingId));
            }
        }
    }
}
