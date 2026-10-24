using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed record HostExperienceListItem(
    int Id,
    string Title,
    string Description,
    decimal PricePerPerson,
    int DurationMinutes,
    string ExperienceType,
    string CulturalArea,
    ExperienceStatus Status,
    string? RejectionReason,
    DateTime CreatedAt,
    int? CoverMediaId);
