using DiscoverEgypt.Application.Contracts.Notifications;
using DiscoverEgypt.Application.Enums.Notifications;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Dashboard.Requests.Queries;
using DiscoverEgypt.Application.Features.Recommendations.Requests.Queries;
using DiscoverEgypt.Application.Features.TouristPreferences.Requests.Queries;
using DiscoverEgypt.Application.Responses.Dashboard;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using MediatR;

namespace DiscoverEgypt.Application.Features.Dashboard.Handlers.Queries
{
    public sealed class GetTouristDashboardQueryHandler : IRequestHandler<GetTouristDashboardQuery, TouristDashboardResponse>
    {
        private readonly ISender _sender;
        private readonly INotificationService _notificationService;
        private readonly TimeProvider _timeProvider;

        public GetTouristDashboardQueryHandler(
            ISender sender,
            INotificationService notificationService,
            TimeProvider timeProvider)
        {
            _sender = sender;
            _notificationService = notificationService;
            _timeProvider = timeProvider;
        }

        public async Task<TouristDashboardResponse> Handle(
            GetTouristDashboardQuery request,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);

            var bookings = await _sender.Send(new GetTouristBookingsQuery(request.UserId), cancellationToken);
            var preferences = await _sender.Send(new GetTouristPreferenceQuery(request.UserId), cancellationToken);
            var recommendations = await _sender.Send(new GetRecommendedExperiencesQuery(request.UserId), cancellationToken);
            var notifications = await _notificationService.GetForUserAsync(
                request.UserId,
                NotificationReadFilter.All,
                page: 1,
                pageSize: 5,
                cancellationToken);

            var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
            var upcomingBooking = bookings
                .Where(booking => booking.BookingStatus == BookingStatus.Confirmed
                    && booking.PaymentStatus == PaymentStatus.Paid
                    && booking.ScheduledStartUtc > nowUtc)
                .OrderBy(booking => booking.ScheduledStartUtc)
                .ThenBy(booking => booking.BookingId)
                .FirstOrDefault();

            return new TouristDashboardResponse(
                upcomingBooking,
                bookings.Take(3).ToArray(),
                preferences,
                recommendations,
                notifications);
        }
    }
}
