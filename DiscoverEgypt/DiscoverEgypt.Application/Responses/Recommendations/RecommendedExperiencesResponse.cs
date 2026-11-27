using DiscoverEgypt.Application.DTOs.Experiences;
using DiscoverEgypt.Application.DTOs.Recommendations;

namespace DiscoverEgypt.Application.Responses.Recommendations
{
    public sealed record RecommendedExperiencesResponse(
        IReadOnlyList<RecommendedExperienceDto> Items,
        IReadOnlyList<ExperienceFilterOptionDto> Types,
        int? SelectedTypeId,
        bool HasPreferences,
        bool UsesSavedTypePreferences,
        IReadOnlyList<string> RequiredLanguages);
}
