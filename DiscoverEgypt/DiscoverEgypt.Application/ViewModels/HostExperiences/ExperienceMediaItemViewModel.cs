
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.ViewModels.HostExperiences
{
    public sealed record ExperienceMediaItemViewModel(
        int Id,
        MediaType MediaType,
        string ContentType,
        int DisplayOrder,
        bool IsCover);
}

