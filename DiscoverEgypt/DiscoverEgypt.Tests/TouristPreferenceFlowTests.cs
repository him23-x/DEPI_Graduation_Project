using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.TouristPreferences;
using DiscoverEgypt.Application.Features.TouristPreferences.Handlers.Commands;
using DiscoverEgypt.Application.Features.TouristPreferences.Handlers.Queries;
using DiscoverEgypt.Application.Features.TouristPreferences.Requests.Commands;
using DiscoverEgypt.Application.Features.TouristPreferences.Requests.Queries;
using DiscoverEgypt.Application.Features.TouristPreferences.Validators;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Preferences;

namespace DiscoverEgypt.Tests
{
    public sealed class TouristPreferenceFlowTests
    {
        [Fact]
        public async Task SaveAndReload_PersistsAllPreferenceValuesForTheOwner()
        {
            var repository = new InMemoryTouristPreferenceRepository();
            var handler = new SaveTouristPreferenceCommandHandler(repository);
            var form = new TouristPreferenceFormDto
            {
                Budget = Budget.From800To1500,
                Duration = DurationPreference.From2To4h,
                GuidePreference = GuidePreference.Preferred,
                InterestIds = [1, 2],
                ExperienceTypeIds = [1],
                CulturalAreaIds = [2],
                LanguageIds = [1, 2]
            };

            await handler.Handle(new SaveTouristPreferenceCommand("tourist-one", form), CancellationToken.None);
            var page = await new GetTouristPreferenceQueryHandler(repository)
                .Handle(new GetTouristPreferenceQuery("tourist-one"), CancellationToken.None);

            Assert.Equal(Budget.From800To1500, page.Budget);
            Assert.Equal(DurationPreference.From2To4h, page.Duration);
            Assert.Equal(GuidePreference.Preferred, page.GuidePreference);
            Assert.Equal([1, 2], page.InterestIds.Order());
            Assert.Equal([1], page.ExperienceTypeIds);
            Assert.Equal([2], page.CulturalAreaIds);
            Assert.Equal([1, 2], page.LanguageIds.Order());
        }

        [Fact]
        public async Task Validator_RejectsLookupIdsThatDoNotExist()
        {
            var validator = new SaveTouristPreferenceCommandValidator(new InMemoryTouristPreferenceRepository());
            var command = new SaveTouristPreferenceCommand("tourist-one", new TouristPreferenceFormDto
            {
                Budget = Budget.Under500,
                Duration = DurationPreference.Under2h,
                GuidePreference = GuidePreference.Either,
                InterestIds = [999]
            });

            var result = await validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.ErrorMessage.Contains("no longer available", StringComparison.Ordinal));
        }

        [Fact]
        public async Task Preferences_AreLoadedOnlyForTheRequestedTourist()
        {
            var repository = new InMemoryTouristPreferenceRepository();
            var handler = new SaveTouristPreferenceCommandHandler(repository);
            await handler.Handle(new SaveTouristPreferenceCommand("tourist-one", new TouristPreferenceFormDto
            {
                Budget = Budget.Under500,
                Duration = DurationPreference.Under2h,
                GuidePreference = GuidePreference.NoGuide,
                InterestIds = [1]
            }), CancellationToken.None);
            await handler.Handle(new SaveTouristPreferenceCommand("tourist-two", new TouristPreferenceFormDto
            {
                Budget = Budget.Over3000,
                Duration = DurationPreference.FullDay,
                GuidePreference = GuidePreference.Preferred,
                InterestIds = [2]
            }), CancellationToken.None);

            var query = new GetTouristPreferenceQueryHandler(repository);
            var first = await query.Handle(new GetTouristPreferenceQuery("tourist-one"), CancellationToken.None);
            var second = await query.Handle(new GetTouristPreferenceQuery("tourist-two"), CancellationToken.None);

            Assert.Equal(Budget.Under500, first.Budget);
            Assert.Equal([1], first.InterestIds);
            Assert.Equal(Budget.Over3000, second.Budget);
            Assert.Equal([2], second.InterestIds);
        }

        private sealed class InMemoryTouristPreferenceRepository : ITouristPreferenceRepository
        {
            private readonly Dictionary<string, TouristPreference> _preferences = new();
            private readonly List<Interest> _interests = [new() { Id = 1, Name = "History" }, new() { Id = 2, Name = "Food" }];
            private readonly List<ExperienceType> _experienceTypes = [new() { Id = 1, Name = "Workshop" }, new() { Id = 2, Name = "Walking tour" }];
            private readonly List<CulturalArea> _areas = [new() { Id = 1, Name = "Cairo" }, new() { Id = 2, Name = "Luxor" }];
            private readonly List<Language> _languages = [new() { Id = 1, Name = "English" }, new() { Id = 2, Name = "Arabic" }];

            public Task<TouristPreference?> GetForUserAsync(string userId, CancellationToken cancellationToken = default) =>
                Task.FromResult(_preferences.TryGetValue(userId, out var preference) ? Clone(preference) : null);

            public Task<IReadOnlyList<Interest>> GetInterestsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Interest>>(_interests);
            public Task<IReadOnlyList<ExperienceType>> GetExperienceTypesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ExperienceType>>(_experienceTypes);
            public Task<IReadOnlyList<CulturalArea>> GetCulturalAreasAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<CulturalArea>>(_areas);
            public Task<IReadOnlyList<Language>> GetLanguagesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Language>>(_languages);

            public Task<bool> AreLookupSelectionsValidAsync(
                IReadOnlyCollection<int> interestIds,
                IReadOnlyCollection<int> experienceTypeIds,
                IReadOnlyCollection<int> culturalAreaIds,
                IReadOnlyCollection<int> languageIds,
                CancellationToken cancellationToken = default) => Task.FromResult(
                    interestIds.All(id => _interests.Any(item => item.Id == id))
                    && experienceTypeIds.All(id => _experienceTypes.Any(item => item.Id == id))
                    && culturalAreaIds.All(id => _areas.Any(item => item.Id == id))
                    && languageIds.All(id => _languages.Any(item => item.Id == id)));

            public Task SaveAsync(
                string userId,
                Budget budget,
                DurationPreference duration,
                GuidePreference guidePreference,
                IReadOnlyCollection<int> interestIds,
                IReadOnlyCollection<int> experienceTypeIds,
                IReadOnlyCollection<int> culturalAreaIds,
                IReadOnlyCollection<int> languageIds,
                CancellationToken cancellationToken = default)
            {
                _preferences[userId] = new TouristPreference
                {
                    UserId = userId,
                    Budget = budget,
                    Duration = duration,
                    GuidePreference = guidePreference,
                    PreferenceInterests = interestIds.Select(id => new PreferenceInterest { InterestId = id }).ToList(),
                    PreferenceExperienceTypes = experienceTypeIds.Select(id => new PreferenceExperienceType { ExperienceTypeId = id }).ToList(),
                    PreferenceCulturalAreas = culturalAreaIds.Select(id => new PreferenceCulturalArea { CulturalAreaId = id }).ToList(),
                    PreferenceLanguages = languageIds.Select(id => new PreferenceLanguage { LanguageId = id }).ToList()
                };
                return Task.CompletedTask;
            }

            private static TouristPreference Clone(TouristPreference preference) => new()
            {
                UserId = preference.UserId,
                Budget = preference.Budget,
                Duration = preference.Duration,
                GuidePreference = preference.GuidePreference,
                PreferenceInterests = preference.PreferenceInterests.Select(link => new PreferenceInterest { InterestId = link.InterestId }).ToList(),
                PreferenceExperienceTypes = preference.PreferenceExperienceTypes.Select(link => new PreferenceExperienceType { ExperienceTypeId = link.ExperienceTypeId }).ToList(),
                PreferenceCulturalAreas = preference.PreferenceCulturalAreas.Select(link => new PreferenceCulturalArea { CulturalAreaId = link.CulturalAreaId }).ToList(),
                PreferenceLanguages = preference.PreferenceLanguages.Select(link => new PreferenceLanguage { LanguageId = link.LanguageId }).ToList()
            };
        }
    }
}
