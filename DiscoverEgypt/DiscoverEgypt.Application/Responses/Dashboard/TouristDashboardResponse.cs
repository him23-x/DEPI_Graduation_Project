using DiscoverEgypt.Application.DTOs.TouristPreferences;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Application.Responses.Recommendations;
using DiscoverEgypt.Application.Responses.Notifications;

namespace DiscoverEgypt.Application.Responses.Dashboard
{
    public sealed record TouristDashboardResponse(
        TouristBookingListItemResponse? UpcomingBooking,
        IReadOnlyList<TouristBookingListItemResponse> RecentBookings,
        TouristPreferenceFormDto Preferences,
        RecommendedExperiencesResponse Recommendations,
        NotificationsResponse RecentNotifications);
}
