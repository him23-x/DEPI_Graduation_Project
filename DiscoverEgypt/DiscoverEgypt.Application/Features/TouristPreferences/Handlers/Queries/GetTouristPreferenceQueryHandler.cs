using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.TouristPreferences;
using DiscoverEgypt.Application.Features.TouristPreferences.Requests.Queries;
using DiscoverEgypt.Domain.Enums.Preferences;
using MediatR;

namespace DiscoverEgypt.Application.Features.TouristPreferences.Handlers.Queries
{
    public sealed class GetTouristPreferenceQueryHandler : IRequestHandler<GetTouristPreferenceQuery, TouristPreferenceFormDto>
    {
        private readonly ITouristPreferenceRepository _repository;

        public GetTouristPreferenceQueryHandler(ITouristPreferenceRepository repository) => _repository = repository;

        public async Task<TouristPreferenceFormDto> Handle(GetTouristPreferenceQuery request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);

            var preference = await _repository.GetForUserAsync(request.UserId, cancellationToken);
            var interests = await _repository.GetInterestsAsync(cancellationToken);
            var experienceTypes = await _repository.GetExperienceTypesAsync(cancellationToken);
            var culturalAreas = await _repository.GetCulturalAreasAsync(cancellationToken);
            var languages = await _repository.GetLanguagesAsync(cancellationToken);

            return new TouristPreferenceFormDto
            {
                Budget = preference?.Budget,
                Duration = preference?.Duration,
                GuidePreference = preference?.GuidePreference,
                InterestIds = preference?.PreferenceInterests.Select(link => link.InterestId).ToList() ?? [],
                ExperienceTypeIds = preference?.PreferenceExperienceTypes.Select(link => link.ExperienceTypeId).ToList() ?? [],
                CulturalAreaIds = preference?.PreferenceCulturalAreas.Select(link => link.CulturalAreaId).ToList() ?? [],
                LanguageIds = preference?.PreferenceLanguages.Select(link => link.LanguageId).ToList() ?? [],
                BudgetOptions = Enum.GetValues<Budget>().Select(value => new PreferenceChoiceDto(value.ToString(), GetBudgetLabel(value))).ToArray(),
                DurationOptions = Enum.GetValues<DurationPreference>().Select(value => new PreferenceChoiceDto(value.ToString(), GetDurationLabel(value))).ToArray(),
                GuidePreferenceOptions = Enum.GetValues<GuidePreference>().Select(value => new PreferenceChoiceDto(value.ToString(), GetGuidePreferenceLabel(value))).ToArray(),
                Interests = interests.Select(item => new PreferenceOptionDto(item.Id, item.Name)).ToArray(),
                ExperienceTypes = experienceTypes.Select(item => new PreferenceOptionDto(item.Id, item.Name)).ToArray(),
                CulturalAreas = culturalAreas.Select(item => new PreferenceOptionDto(item.Id, item.Name)).ToArray(),
                Languages = languages.Select(item => new PreferenceOptionDto(item.Id, item.Name)).ToArray()
            };
        }

        private static string GetBudgetLabel(Budget value) => value switch
        {
            Budget.Under500 => "Under EGP 500",
            Budget.From500To800 => "EGP 500–800",
            Budget.From800To1500 => "EGP 800–1,500",
            Budget.From1500To3000 => "EGP 1,500–3,000",
            Budget.Over3000 => "Over EGP 3,000",
            _ => value.ToString()
        };

        private static string GetDurationLabel(DurationPreference value) => value switch
        {
            DurationPreference.Under2h => "Under 2 hours",
            DurationPreference.From2To4h => "2–4 hours",
            DurationPreference.From4To6h => "4–6 hours",
            DurationPreference.FullDay => "Full day",
            _ => value.ToString()
        };

        private static string GetGuidePreferenceLabel(GuidePreference value) => value switch
        {
            GuidePreference.Preferred => "A guide is preferred",
            GuidePreference.NoGuide => "No guide needed",
            GuidePreference.Either => "No preference",
            _ => value.ToString()
        };
    }
}
