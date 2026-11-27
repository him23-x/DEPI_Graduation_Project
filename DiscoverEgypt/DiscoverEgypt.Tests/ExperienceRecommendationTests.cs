using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Experiences;
using DiscoverEgypt.Application.DTOs.Recommendations;
using DiscoverEgypt.Application.Features.Recommendations.Handlers.Queries;
using DiscoverEgypt.Application.Features.Recommendations.Requests.Queries;
using DiscoverEgypt.Application.Responses.Recommendations;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Experiences;
using DiscoverEgypt.Domain.Enums.Preferences;

namespace DiscoverEgypt.Tests
{
    public sealed class ExperienceRecommendationTests
    {
        [Fact]
        public async Task Recommendations_UseProportionalInterestMatch_AndRenormalizeWhenNoReviewsExist()
        {
            var repository = new InMemoryRecommendationRepository(CreatePreference(languageIds: []),
            [
                Candidate(1, "Perfect match", 1000m, 180, 4.5m, 2, [1, 2], [1]),
                Candidate(2, "Partial match", 1000m, 180, 0m, 0, [1], [1]),
                Candidate(99, "Unpublished draft", 1000m, 180, 5m, 10, [1, 2], [1], status: ExperienceStatus.Draft)
            ]);
            var handler = CreateHandler(repository);

            var response = await handler.Handle(new GetRecommendedExperiencesQuery("tourist-1"), CancellationToken.None);

            Assert.Equal(2, response.Items.Count);
            Assert.Equal(99m, response.Items[0].MatchPercentage);
            Assert.Equal(77.8m, response.Items[1].MatchPercentage);
            Assert.Contains(response.Items[1].MatchDetails, detail => detail.Contains("1 of 2", StringComparison.Ordinal));
            Assert.Contains(response.Items[1].MatchDetails, detail => detail.Contains("reweighted", StringComparison.Ordinal));
        }

        [Fact]
        public async Task Recommendations_RequireOneOfTheSelectedLanguages()
        {
            var repository = new InMemoryRecommendationRepository(CreatePreference(languageIds: [1]),
            [
                Candidate(1, "English experience", 1000m, 180, 4m, 1, [1], [1]),
                Candidate(2, "Arabic experience", 1000m, 180, 5m, 4, [1], [2])
            ]);

            var response = await CreateHandler(repository)
                .Handle(new GetRecommendedExperiencesQuery("tourist-1"), CancellationToken.None);

            Assert.Single(response.Items);
            Assert.Equal(1, response.Items[0].Id);
            Assert.Equal(["English"], response.RequiredLanguages);
        }

        [Fact]
        public async Task BudgetDurationAndArea_AreScoredAsRangeAndCategoryMatches()
        {
            var preference = CreatePreference(languageIds: []);
            preference.PreferenceInterests = [];
            preference.PreferenceCulturalAreas = [new PreferenceCulturalArea { CulturalAreaId = 5 }];
            var repository = new InMemoryRecommendationRepository(preference,
            [
                Candidate(10, "At lower boundaries", 800m, 120, 0m, 0, [], []),
                Candidate(11, "Outside selected ranges", 1500m, 360, 0m, 0, [], [], areaId: 6)
            ]);

            var response = await CreateHandler(repository)
                .Handle(new GetRecommendedExperiencesQuery("tourist-1"), CancellationToken.None);

            Assert.Equal(100m, response.Items[0].MatchPercentage);
            Assert.Equal(0m, response.Items[1].MatchPercentage);
            Assert.Contains(response.Items[0].MatchDetails, detail => detail == "Within your budget");
            Assert.Contains(response.Items[0].MatchDetails, detail => detail == "Matches your preferred duration");
            Assert.Contains(response.Items[1].MatchDetails, detail => detail == "Outside your preferred regions");
        }

        [Fact]
        public async Task ExplicitTypeFilter_OverridesSavedTypePreferences()
        {
            var repository = new InMemoryRecommendationRepository(CreatePreference(languageIds: [], typeIds: [3]),
            [
                Candidate(1, "Saved type", 1000m, 180, 4m, 1, [1], [1], typeId: 3),
                Candidate(2, "Chosen type", 1000m, 180, 4m, 1, [1], [1], typeId: 4)
            ]);

            var defaultResponse = await CreateHandler(repository)
                .Handle(new GetRecommendedExperiencesQuery("tourist-1"), CancellationToken.None);
            var filteredResponse = await CreateHandler(repository)
                .Handle(new GetRecommendedExperiencesQuery("tourist-1", 4), CancellationToken.None);

            Assert.True(defaultResponse.UsesSavedTypePreferences);
            Assert.Equal([1], defaultResponse.Items.Select(item => item.Id));
            Assert.False(filteredResponse.UsesSavedTypePreferences);
            Assert.Equal(4, filteredResponse.SelectedTypeId);
            Assert.Equal([2], filteredResponse.Items.Select(item => item.Id));
        }

        [Fact]
        public async Task Recommendations_RequirePreferences_AndCandidateContractHasNoExactLocation()
        {
            var repository = new InMemoryRecommendationRepository(null, [Candidate(1, "Public", 1000m, 180, 4m, 1, [1], [1])]);
            var response = await CreateHandler(repository)
                .Handle(new GetRecommendedExperiencesQuery("tourist-without-preferences"), CancellationToken.None);

            Assert.False(response.HasPreferences);
            Assert.Empty(response.Items);
            Assert.Null(typeof(RecommendationExperienceCandidateDto).GetProperty("ExactMeetingPoint"));
        }

        private static GetRecommendedExperiencesQueryHandler CreateHandler(InMemoryRecommendationRepository repository) =>
            new(repository, repository);

        private static TouristPreference CreatePreference(
            int[]? languageIds = null,
            int[]? typeIds = null) => new()
        {
            UserId = "tourist-1",
            Budget = Budget.From800To1500,
            Duration = DurationPreference.From2To4h,
            GuidePreference = GuidePreference.Either,
            PreferenceInterests = [new PreferenceInterest { InterestId = 1 }, new PreferenceInterest { InterestId = 2 }],
            PreferenceCulturalAreas = [new PreferenceCulturalArea { CulturalAreaId = 5 }],
            PreferenceLanguages = (languageIds ?? [1]).Select(id => new PreferenceLanguage
            {
                LanguageId = id,
                Language = new Language { Id = id, Name = id == 1 ? "English" : "Arabic" }
            }).ToList(),
            PreferenceExperienceTypes = (typeIds ?? []).Select(id => new PreferenceExperienceType { ExperienceTypeId = id }).ToList()
        };

        private static RecommendationExperienceCandidateDto Candidate(
            int id,
            string title,
            decimal price,
            int duration,
            decimal rating,
            int reviewCount,
            int[] interestIds,
            int[] languageIds,
            int typeId = 3,
            ExperienceStatus status = ExperienceStatus.Published,
            int areaId = 5) => new(
                id, status, title, "Workshop", typeId, "Cairo", areaId, "Local host", null, "A cultural story.",
                price, duration, rating, reviewCount, interestIds, languageIds);

        private sealed class InMemoryRecommendationRepository : ITouristPreferenceRepository, IExperienceExploreRepository
        {
            private readonly TouristPreference? _preference;
            private readonly IReadOnlyList<RecommendationExperienceCandidateDto> _candidates;

            public InMemoryRecommendationRepository(
                TouristPreference? preference,
                IReadOnlyList<RecommendationExperienceCandidateDto> candidates)
            {
                _preference = preference;
                _candidates = candidates;
            }

            public Task<TouristPreference?> GetForUserAsync(string userId, CancellationToken cancellationToken = default) =>
                Task.FromResult(_preference is not null && _preference.UserId == userId ? _preference : null);

            public Task<IReadOnlyList<Interest>> GetInterestsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Interest>>([]);
            public Task<IReadOnlyList<ExperienceType>> GetExperienceTypesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ExperienceType>>([]);
            public Task<IReadOnlyList<CulturalArea>> GetCulturalAreasAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<CulturalArea>>([]);
            public Task<IReadOnlyList<Language>> GetLanguagesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Language>>([]);
            public Task<bool> AreLookupSelectionsValidAsync(IReadOnlyCollection<int> interestIds, IReadOnlyCollection<int> experienceTypeIds, IReadOnlyCollection<int> culturalAreaIds, IReadOnlyCollection<int> languageIds, CancellationToken cancellationToken = default) => Task.FromResult(true);
            public Task SaveAsync(string userId, Budget budget, DurationPreference duration, GuidePreference guidePreference, IReadOnlyCollection<int> interestIds, IReadOnlyCollection<int> experienceTypeIds, IReadOnlyCollection<int> culturalAreaIds, IReadOnlyCollection<int> languageIds, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task<IReadOnlyList<RecommendationExperienceCandidateDto>> GetRecommendationCandidatesAsync(CancellationToken cancellationToken = default) =>
                Task.FromResult(_candidates);

            public Task<IReadOnlyList<ExploreExperienceDto>> GetPageAsync(string search, int? typeId, int? areaId, decimal? minPrice, decimal? maxPrice, string sort, int skip, int take, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ExploreExperienceDto>>([]);
            public Task<int> CountAsync(string search, int? typeId, int? areaId, decimal? minPrice, decimal? maxPrice, CancellationToken cancellationToken = default) => Task.FromResult(0);
            public Task<IReadOnlyList<ExperienceFilterOptionDto>> GetTypesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ExperienceFilterOptionDto>>([new(3, "Workshop"), new(4, "Walking tour")]);
            public Task<IReadOnlyList<ExperienceFilterOptionDto>> GetAreasAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ExperienceFilterOptionDto>>([]);
            public Task<string?> GetPublishedImageKeyAsync(int experienceId, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);
        }
    }
}
