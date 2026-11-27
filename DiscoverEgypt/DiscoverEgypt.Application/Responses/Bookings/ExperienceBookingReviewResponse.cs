namespace DiscoverEgypt.Application.Responses.Bookings
{
    public sealed record ExperienceBookingReviewResponse(
        int ExperienceId,
        string Title,
        string ExperienceType,
        string CulturalArea,
        string MeetingPointArea,
        string HostName,
        DateTime ScheduledStartLocal,
        DateTime ScheduledEndLocal,
        int NumberOfGuests,
        decimal PricePerPerson,
        decimal ServiceAmount,
        decimal PlatformFeeRate,
        decimal PlatformFeeAmount,
        decimal TotalAmount,
        int DurationMinutes);
}
