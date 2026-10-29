
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed record ExperienceListItemViewModel(
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
