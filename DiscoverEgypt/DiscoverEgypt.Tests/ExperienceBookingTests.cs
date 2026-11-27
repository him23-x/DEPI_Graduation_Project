using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Services;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using FluentValidation;

namespace DiscoverEgypt.Tests
{
    public sealed class ExperienceBookingTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);

        [Fact]
        public async Task Preview_AcceptsExact72HourBoundary_AndCalculatesTheApprovedFee()
        {
            var repository = new FakeExperienceBookingRepository();
            var service = CreateService(repository);

            var response = await service.PreviewAsync(FormAt(Now.AddHours(72), guests: 2), CancellationToken.None);

            Assert.Equal(1000m, response.ServiceAmount);
            Assert.Equal(0.15m, response.PlatformFeeRate);
            Assert.Equal(150m, response.PlatformFeeAmount);
            Assert.Equal(1150m, response.TotalAmount);
            Assert.Equal(2, response.NumberOfGuests);
            Assert.Equal("Old Cairo", response.MeetingPointArea);
        }

        [Fact]
        public async Task Preview_RejectsStartLessThan72HoursAway()
        {
            var service = CreateService(new FakeExperienceBookingRepository());

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                service.PreviewAsync(FormAt(Now.AddHours(71).AddMinutes(59), guests: 1), CancellationToken.None));

            Assert.Contains("72 hours", exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task Preview_RejectsGuestCountOverExperienceLimit()
        {
            var service = CreateService(new FakeExperienceBookingRepository());

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                service.PreviewAsync(FormAt(Now.AddHours(80), guests: 7), CancellationToken.None));

            Assert.Contains("up to 6 guests", exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task Preview_RejectsUnavailableExperiencesAndConfirmedOverlaps()
        {
            var missingExperience = new FakeExperienceBookingRepository { IsAvailable = false };
            var unavailable = await Assert.ThrowsAsync<ValidationException>(() =>
                CreateService(missingExperience).PreviewAsync(FormAt(Now.AddHours(80), guests: 1), CancellationToken.None));
            Assert.Contains("no longer available", unavailable.Message, StringComparison.Ordinal);

            var overlap = new FakeExperienceBookingRepository { HasOverlap = true };
            var overlapping = await Assert.ThrowsAsync<ValidationException>(() =>
                CreateService(overlap).PreviewAsync(FormAt(Now.AddHours(80), guests: 1), CancellationToken.None));
            Assert.Contains("confirmed booking", overlapping.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task Create_SavesUtcScheduleAndPriceSnapshotsForAuthenticatedTourist()
        {
            var repository = new FakeExperienceBookingRepository();
            var service = CreateService(repository);
            var form = FormAt(Now.AddHours(80), guests: 3);

            var bookingId = await service.CreateAsync("tourist-user-id", form, CancellationToken.None);

            Assert.Equal(45, bookingId);
            var saved = Assert.IsType<Booking>(repository.SavedBooking);
            Assert.Equal("tourist-user-id", saved.TouristUserId);
            Assert.Equal("host-user-id", saved.ProviderUserId);
            Assert.Equal(BookingType.Experience, saved.BookingType);
            Assert.Equal(DateTimeKind.Utc, saved.ScheduledStart.Kind);
            Assert.Equal(DateTimeKind.Utc, saved.ScheduledEnd.Kind);
            Assert.Equal(1500m, saved.ServiceAmount);
            Assert.Equal(225m, saved.PlatformFeeAmount);
            Assert.Equal(1725m, saved.TotalAmount);
            Assert.Equal(BookingStatus.Pending, saved.Status);
            Assert.Equal(PaymentStatus.Pending, saved.Payment.Status);
            Assert.Equal(saved.TotalAmount, saved.Payment.Amount);
        }

        [Fact]
        public async Task Create_RejectsActiveTouristBanWithoutSavingBooking()
        {
            var repository = new FakeExperienceBookingRepository();
            var service = CreateService(repository, isBanned: true);

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                service.CreateAsync("tourist-user-id", FormAt(Now.AddHours(80), guests: 1), CancellationToken.None));

            Assert.Contains("active restriction", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(repository.SavedBooking);
        }

        [Fact]
        public void BookingFormAndReviewContracts_DoNotExposeExactMeetingPoint()
        {
            Assert.Null(typeof(ExperienceBookingDataDto).GetProperty("ExactMeetingPoint"));
            Assert.Null(typeof(ExperienceBookingFormResponse).GetProperty("ExactMeetingPoint"));
            Assert.Null(typeof(ExperienceBookingReviewResponse).GetProperty("ExactMeetingPoint"));
            Assert.Null(typeof(ExperienceBookingConfirmationResponse).GetProperty("ExactMeetingPoint"));
        }

        private static ExperienceBookingService CreateService(FakeExperienceBookingRepository repository, bool isBanned = false) =>
            new(repository, new FixedTimeProvider(Now), new FakeTouristSanctionRepository(isBanned));

        private static ExperienceBookingInputDto FormAt(DateTimeOffset startUtc, int guests)
        {
            var cairo = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "Egypt Standard Time" : "Africa/Cairo");
            var localStart = TimeZoneInfo.ConvertTime(startUtc, cairo).DateTime;
            return new ExperienceBookingInputDto
            {
                ExperienceId = 12,
                NumberOfGuests = guests,
                ScheduledStartLocal = DateTime.SpecifyKind(localStart, DateTimeKind.Unspecified)
            };
        }

        private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
        }

        private sealed class FakeTouristSanctionRepository(bool isBanned) : ITouristSanctionRepository
        {
            public Task<bool> HasActiveTouristBanAsync(string touristUserId, DateTime nowUtc,
                CancellationToken cancellationToken = default) => Task.FromResult(isBanned);
        }

        private sealed class FakeExperienceBookingRepository : IExperienceBookingRepository
        {
            public bool IsAvailable { get; init; } = true;
            public bool HasOverlap { get; init; }
            public Booking? SavedBooking { get; private set; }

            public Task<ExperienceBookingDataDto?> GetPublishedExperienceAsync(int experienceId, CancellationToken cancellationToken = default) =>
                Task.FromResult<ExperienceBookingDataDto?>(IsAvailable && experienceId == 12
                    ? new ExperienceBookingDataDto(12, "Cairo Food Walk", "Food & Cooking", "Cairo", "Old Cairo", "Local host",
                        "host-user-id", 500m, 120, 6)
                    : null);

            public Task<bool> HasConfirmedOverlapAsync(string providerUserId, DateTime scheduledStartUtc, DateTime scheduledEndUtc, CancellationToken cancellationToken = default) =>
                Task.FromResult(HasOverlap);

            public Task<int> CreateAsync(Booking booking, CancellationToken cancellationToken = default)
            {
                SavedBooking = booking;
                booking.Id = 45;
                return Task.FromResult(booking.Id);
            }

            public Task<ExperienceBookingConfirmationResponse?> GetTouristConfirmationAsync(string userId, int bookingId, CancellationToken cancellationToken = default) =>
                Task.FromResult<ExperienceBookingConfirmationResponse?>(null);
        }
    }
}
