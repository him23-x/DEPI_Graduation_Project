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
    public sealed class GuideBookingTests
    {
        private static readonly DateTimeOffset Now = new(2026, 11, 23, 12, 0, 0, TimeSpan.Zero);

        [Fact]
        public async Task Preview_AcceptsExact72HourBoundary_AndCalculatesHourlyPriceAndApprovedFee()
        {
            var service = CreateService(new FakeGuideBookingRepository());

            var response = await service.PreviewAsync(FormAt(Now.AddHours(72), hours: 3), CancellationToken.None);

            Assert.Equal(2400m, response.ServiceAmount);
            Assert.Equal(0.15m, response.PlatformFeeRate);
            Assert.Equal(360m, response.PlatformFeeAmount);
            Assert.Equal(2760m, response.TotalAmount);
            Assert.Equal(3, response.GuideHours);
            Assert.Equal("Giza Plateau", response.Destination);
        }

        [Fact]
        public async Task Preview_RejectsStartLessThan72HoursAway()
        {
            var service = CreateService(new FakeGuideBookingRepository());

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                service.PreviewAsync(FormAt(Now.AddHours(71).AddMinutes(59), 2), CancellationToken.None));

            Assert.Contains("72 hours", exception.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task Preview_RejectsUnapprovedOrExpiredGuideAndConfirmedOverlap()
        {
            var unapproved = new FakeGuideBookingRepository { IsApproved = false };
            var unavailable = await Assert.ThrowsAsync<ValidationException>(() =>
                CreateService(unapproved).PreviewAsync(FormAt(Now.AddHours(80), 2), CancellationToken.None));
            Assert.Contains("no longer available", unavailable.Message, StringComparison.Ordinal);

            var expired = new FakeGuideBookingRepository { LicenseExpiryDate = new DateOnly(2026, 11, 22) };
            var expiredLicense = await Assert.ThrowsAsync<ValidationException>(() =>
                CreateService(expired).PreviewAsync(FormAt(Now.AddHours(80), 2), CancellationToken.None));
            Assert.Contains("no longer available", expiredLicense.Message, StringComparison.Ordinal);

            var overlap = new FakeGuideBookingRepository { HasOverlap = true };
            var overlapping = await Assert.ThrowsAsync<ValidationException>(() =>
                CreateService(overlap).PreviewAsync(FormAt(Now.AddHours(80), 2), CancellationToken.None));
            Assert.Contains("confirmed booking", overlapping.Message, StringComparison.Ordinal);
        }

        [Fact]
        public async Task Preview_RejectsMissingDestinationAndNonPositiveDuration()
        {
            var service = CreateService(new FakeGuideBookingRepository());
            var missingDestination = FormAt(Now.AddHours(80), 2);
            missingDestination.Destination = " ";
            var destinationError = await Assert.ThrowsAsync<ValidationException>(() =>
                service.PreviewAsync(missingDestination, CancellationToken.None));
            Assert.Contains("destination", destinationError.Message, StringComparison.OrdinalIgnoreCase);

            var noDuration = FormAt(Now.AddHours(80), 2);
            noDuration.GuideHours = 0;
            var durationError = await Assert.ThrowsAsync<ValidationException>(() =>
                service.PreviewAsync(noDuration, CancellationToken.None));
            Assert.Contains("hour", durationError.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Create_SavesUtcScheduleAndPriceSnapshotsForAuthenticatedTourist()
        {
            var repository = new FakeGuideBookingRepository();
            var service = CreateService(repository);

            var bookingId = await service.CreateAsync("tourist-user-id", FormAt(Now.AddHours(80), 4), CancellationToken.None);

            Assert.Equal(48, bookingId);
            var saved = Assert.IsType<Booking>(repository.SavedBooking);
            Assert.Equal("tourist-user-id", saved.TouristUserId);
            Assert.Equal("guide-user-id", saved.ProviderUserId);
            Assert.Equal(BookingType.Guide, saved.BookingType);
            Assert.Equal(18, saved.GuideProfileId);
            Assert.Equal(4, saved.GuideHours);
            Assert.Equal("Giza Plateau", saved.Destination);
            Assert.Equal(DateTimeKind.Utc, saved.ScheduledStart.Kind);
            Assert.Equal(DateTimeKind.Utc, saved.ScheduledEnd.Kind);
            Assert.Equal(800m, saved.HourlyRateSnapshot);
            Assert.Equal(3200m, saved.ServiceAmount);
            Assert.Equal(0.15m, saved.PlatformFeeRate);
            Assert.Equal(480m, saved.PlatformFeeAmount);
            Assert.Equal(3680m, saved.TotalAmount);
            Assert.Equal(BookingStatus.Pending, saved.Status);
            Assert.Equal(PaymentStatus.Pending, saved.Payment.Status);
            Assert.Equal(saved.TotalAmount, saved.Payment.Amount);
        }

        [Fact]
        public async Task Create_RejectsActiveTouristBanWithoutSavingBooking()
        {
            var repository = new FakeGuideBookingRepository();
            var service = CreateService(repository, isBanned: true);

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                service.CreateAsync("tourist-user-id", FormAt(Now.AddHours(80), 1), CancellationToken.None));

            Assert.Contains("active restriction", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(repository.SavedBooking);
        }

        [Fact]
        public void GuideBookingResponses_DoNotExposeVerificationDocumentsOrLicenseData()
        {
            Assert.Null(typeof(GuideBookingDataDto).GetProperty("VerificationRequests"));
            Assert.Null(typeof(GuideBookingDataDto).GetProperty("Documents"));
            Assert.Null(typeof(GuideBookingConfirmationResponse).GetProperty("LicenseExpiryDate"));
        }

        private static GuideBookingService CreateService(FakeGuideBookingRepository repository, bool isBanned = false) =>
            new(repository, new FixedTimeProvider(Now), new FakeTouristSanctionRepository(isBanned));

        private static GuideBookingInputDto FormAt(DateTimeOffset startUtc, int hours)
        {
            var cairo = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "Egypt Standard Time" : "Africa/Cairo");
            var localStart = TimeZoneInfo.ConvertTime(startUtc, cairo).DateTime;
            return new GuideBookingInputDto
            {
                GuideProfileId = 18,
                ScheduledStartLocal = DateTime.SpecifyKind(localStart, DateTimeKind.Unspecified),
                GuideHours = hours,
                Destination = "Giza Plateau"
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

        private sealed class FakeGuideBookingRepository : IGuideBookingRepository
        {
            public bool IsApproved { get; init; } = true;
            public bool HasOverlap { get; init; }
            public DateOnly LicenseExpiryDate { get; init; } = new(2027, 1, 1);
            public Booking? SavedBooking { get; private set; }

            public Task<GuideBookingDataDto?> GetApprovedGuideAsync(int guideProfileId, DateOnly todayUtc, CancellationToken cancellationToken = default) =>
                Task.FromResult<GuideBookingDataDto?>(IsApproved && LicenseExpiryDate >= todayUtc && guideProfileId == 18
                    ? new GuideBookingDataDto(18, "Amira Hassan", "Local guide", 800m, null,
                        new[] { "English", "Arabic" }, new[] { "Giza" }, 4.8m, 12, "guide-user-id")
                    : null);

            public Task<bool> HasConfirmedOverlapAsync(string providerUserId, DateTime scheduledStartUtc, DateTime scheduledEndUtc, CancellationToken cancellationToken = default) =>
                Task.FromResult(HasOverlap);

            public Task<int> CreateAsync(Booking booking, CancellationToken cancellationToken = default)
            {
                SavedBooking = booking;
                booking.Id = 48;
                return Task.FromResult(booking.Id);
            }

            public Task<GuideBookingConfirmationResponse?> GetTouristConfirmationAsync(string userId, int bookingId, CancellationToken cancellationToken = default) =>
                Task.FromResult<GuideBookingConfirmationResponse?>(null);
        }
    }
}
