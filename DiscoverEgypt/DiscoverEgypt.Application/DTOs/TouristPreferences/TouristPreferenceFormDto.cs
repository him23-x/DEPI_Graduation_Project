using DiscoverEgypt.Domain.Enums.Preferences;

namespace DiscoverEgypt.Application.DTOs.TouristPreferences
{
    public sealed class TouristPreferenceFormDto
    {
        public Budget? Budget { get; set; }
        public DurationPreference? Duration { get; set; }
        public GuidePreference? GuidePreference { get; set; }
        public List<int> InterestIds { get; set; } = [];
        public List<int> ExperienceTypeIds { get; set; } = [];
        public List<int> CulturalAreaIds { get; set; } = [];
        public List<int> LanguageIds { get; set; } = [];
        public IReadOnlyList<PreferenceChoiceDto> BudgetOptions { get; set; } = [];
        public IReadOnlyList<PreferenceChoiceDto> DurationOptions { get; set; } = [];
        public IReadOnlyList<PreferenceChoiceDto> GuidePreferenceOptions { get; set; } = [];
        public IReadOnlyList<PreferenceOptionDto> Interests { get; set; } = [];
        public IReadOnlyList<PreferenceOptionDto> ExperienceTypes { get; set; } = [];
        public IReadOnlyList<PreferenceOptionDto> CulturalAreas { get; set; } = [];
        public IReadOnlyList<PreferenceOptionDto> Languages { get; set; } = [];
    }
}
