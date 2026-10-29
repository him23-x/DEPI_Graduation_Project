
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed record ExperienceMediaItemViewModel(
    int Id,
    MediaType MediaType,
    string ContentType,
    int DisplayOrder,
    bool IsCover);
