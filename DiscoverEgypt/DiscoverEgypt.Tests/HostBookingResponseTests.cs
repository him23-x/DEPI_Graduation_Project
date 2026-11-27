using AutoMapper;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Enums.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Handlers.Queries;
using DiscoverEgypt.Application.Features.Bookings.Options;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Bookings.Validators;
using DiscoverEgypt.Application.Mapping;
using DiscoverEgypt.Application.Responses.Accounts;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscoverEgypt.Tests;

public sealed class HostBookingResponseTests
{
    [Fact]
    public async Task ListHandler_ReturnsResponseAndPassesHostIdToRepository()
    {
        var paidAt = DateTime.UtcNow.AddHours(-2);
        var booking = CreateBooking(paidAt);
        var repository = new FakeHostBookingRepository(booking);
        var handler = CreateListHandler(repository);

        var result = await handler.Handle(
            new GetHostBookingsQuery("host-789", HostBookingTab.Pending),
            CancellationToken.None);

        Assert.Equal("host-789", repository.ListHostId);
        Assert.Equal("tourist-1", Assert.Single(repository.RequestedTouristIds));
        var item = Assert.Single(result);
        Assert.Equal(booking.Id, item.Id);
        Assert.Equal("Guest Name", item.TouristName);
        Assert.Equal("Food Tour", item.ExperienceTitle);
        Assert.Equal(2, item.NumberOfGuests);
        Assert.Equal(paidAt.AddHours(12), item.ResponseDeadline);
    }

    [Fact]
    public async Task DetailsHandler_ReturnsResponseWithCurrentEligibilityAndPassesHostId()
    {
        var paidAt = DateTime.UtcNow.AddHours(-1);
        var booking = CreateBooking(paidAt);
        var repository = new FakeHostBookingRepository(booking);
        var handler = CreateDetailsHandler(repository);

        var result = await handler.Handle(
            new GetHostBookingDetailsQuery("host-789", booking.Id),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("host-789", repository.DetailsHostId);
        Assert.Equal("tourist-1", Assert.Single(repository.RequestedTouristIds));
        Assert.Equal("host-789", repository.SuspensionHostId);
        Assert.Equal("host-789", repository.OverlapHostId);
        Assert.Equal("Guest Name", result.TouristName);
        Assert.Equal("guest@example.com", result.TouristEmail);
        Assert.Equal("Food Tour", result.ExperienceTitle);
        Assert.Equal("Old Cairo", result.MeetingPointArea);
        Assert.Equal("Exact meeting point", result.ExactMeetingPoint);
        Assert.Equal(paidAt.AddHours(12), result.ResponseDeadline);
        Assert.True(result.CanAccept);
        Assert.True(result.CanReject);
        Assert.False(result.CanComplete);
    }

    private static GetHostBookingsQueryHandler CreateListHandler(FakeHostBookingRepository repository)
    {
        using var loggerFactory = LoggerFactory.Create(_ => { });
        var mapper = new MapperConfiguration(
            configuration => configuration.AddProfile<HostBookingsMappingProfile>(),
            loggerFactory).CreateMapper();
        return new GetHostBookingsQueryHandler(
            repository,
            Options.Create(new BookingLifecycleOptions()),
            mapper,
            new GetHostBookingsQueryValidator());
    }

    private static GetHostBookingDetailsQueryHandler CreateDetailsHandler(FakeHostBookingRepository repository)
    {
        using var loggerFactory = LoggerFactory.Create(_ => { });
        var mapper = new MapperConfiguration(
            configuration => configuration.AddProfile<HostBookingsMappingProfile>(),
            loggerFactory).CreateMapper();
        return new GetHostBookingDetailsQueryHandler(
            repository,
            Options.Create(new BookingLifecycleOptions()),
            mapper,
            new GetHostBookingDetailsQueryValidator());
    }

    private static Booking CreateBooking(DateTime paidAt) => new()
    {
        Id = 32,
        TouristUserId = "tourist-1",
        ProviderUserId = "host-789",
        ScheduledStart = DateTime.UtcNow.AddDays(3),
        ScheduledEnd = DateTime.UtcNow.AddDays(3).AddHours(2),
        NumberOfGuests = 2,
        ServiceAmount = 500m,
        PlatformFeeAmount = 75m,
        TotalAmount = 575m,
        Status = BookingStatus.Pending,
        CreatedAt = DateTime.UtcNow.AddHours(-2),
        Payment = new Payment { Status = PaymentStatus.Paid, PaidAt = paidAt },
        Experience = new Experience
        {
            Title = "Food Tour",
            MeetingPointArea = "Old Cairo",
            ExactMeetingPoint = "Exact meeting point"
        }
    };

    private sealed class FakeHostBookingRepository(Booking booking) : IHostBookingRepository
    {
        public string? ListHostId { get; private set; }
        public string? DetailsHostId { get; private set; }
        public List<string> RequestedTouristIds { get; } = [];
        public string? SuspensionHostId { get; private set; }
        public string? OverlapHostId { get; private set; }

        public Task<IReadOnlyList<Booking>> GetForHostAsync(
            string userId,
            HostBookingTab tab,
            CancellationToken cancellationToken = default)
        {
            ListHostId = userId;
            return Task.FromResult<IReadOnlyList<Booking>>([booking]);
        }

        public Task<Booking?> GetDetailsAsync(string userId, int bookingId, CancellationToken cancellationToken = default)
        {
            DetailsHostId = userId;
            return Task.FromResult<Booking?>(bookingId == booking.Id ? booking : null);
        }

        public Task<IReadOnlyDictionary<string, UserSummaryResponse>> GetTouristsAsync(
            IEnumerable<string> userIds,
            CancellationToken cancellationToken = default)
        {
            RequestedTouristIds.AddRange(userIds);
            return Task.FromResult<IReadOnlyDictionary<string, UserSummaryResponse>>(
                new Dictionary<string, UserSummaryResponse>
                {
                    [booking.TouristUserId] = new("tourist-1", "guest-user", "guest@example.com", "Guest Name")
                });
        }

        public Task<bool> IsProviderSuspendedAsync(
            string userId,
            DateTime bookingCreatedAtUtc,
            DateTime nowUtc,
            CancellationToken cancellationToken = default)
        {
            SuspensionHostId = userId;
            return Task.FromResult(false);
        }

        public Task<bool> HasConfirmedOverlapAsync(
            string userId,
            DateTime scheduledStart,
            DateTime scheduledEnd,
            int exceptBookingId,
            CancellationToken cancellationToken = default)
        {
            OverlapHostId = userId;
            return Task.FromResult(false);
        }

        public Task<HostBookingActionOutcome> ExecuteActionAsync(
            string userId,
            int bookingId,
            Func<HostBookingActionContext, CancellationToken, Task<HostBookingActionOutcome>> action,
            CancellationToken cancellationToken = default) => Task.FromResult(HostBookingActionOutcome.Success);
    }
}
