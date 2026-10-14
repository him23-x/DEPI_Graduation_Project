using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Experiences;

// DTO = the shape sent to the frontend. We never send Domain entities directly.
public record ExperienceDto(
    Guid Id,
    string Title,
    string Description,
    decimal PricePerPerson,
    int DurationMinutes,
    int MaxGroupSize,
    string MeetingPoint)
{
    public static ExperienceDto From(Experience e) => new(
        e.Id, e.Title, e.Description, e.PricePerPerson,
        e.DurationMinutes, e.MaxGroupSize, e.MeetingPoint);
}
