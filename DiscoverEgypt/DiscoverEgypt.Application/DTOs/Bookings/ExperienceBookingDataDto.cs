namespace DiscoverEgypt.Application.DTOs.Bookings
{
    public sealed record ExperienceBookingDataDto(
        int ExperienceId,
        string Title,
        string ExperienceType,
        string CulturalArea,
        string MeetingPointArea,
        string HostName,
        string ProviderUserId,
        decimal PricePerPerson,
        int DurationMinutes,
        int MaxGroupSize);
}
