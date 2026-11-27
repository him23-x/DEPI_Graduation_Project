using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Recommendations;
using DiscoverEgypt.Application.Features.Recommendations.Requests.Queries;
using DiscoverEgypt.Application.Responses.Recommendations;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Experiences;
using DiscoverEgypt.Domain.Enums.Preferences;
using MediatR;

namespace DiscoverEgypt.Application.Features.Recommendations.Handlers.Queries
{
    public sealed class GetRecommendedExperiencesQueryHandler
        : IRequestHandler<GetRecommendedExperiencesQuery, RecommendedExperiencesResponse>
    {
        private const decimal InterestWeight = 40m;
        private const decimal BudgetWeight = 25m;
        private const decimal DurationWeight = 15m;
        private const decimal AreaWeight = 10m;
        private const decimal RatingWeight = 10m;

        private readonly ITouristPreferenceRepository _preferenceRepository;
        private readonly IExperienceExploreRepository _experienceRepository;

        public GetRecommendedExperiencesQueryHandler(
            ITouristPreferenceRepository preferenceRepository,
            IExperienceExploreRepository experienceRepository)
        {
            _preferenceRepository = preferenceRepository;
            _experienceRepository = experienceRepository;
        }

        public async Task<RecommendedExperiencesResponse> Handle(
            GetRecommendedExperiencesQuery request,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);

            var types = await _experienceRepository.GetTypesAsync(cancellationToken);
            var preference = await _preferenceRepository.GetForUserAsync(request.UserId, cancellationToken);
            if (preference is null)
                return new RecommendedExperiencesResponse([], types, request.TypeId, false, false, []);

            var savedTypeIds = preference.PreferenceExperienceTypes.Select(link => link.ExperienceTypeId).Distinct().ToArray();
            var usesSavedTypes = !request.TypeId.HasValue && savedTypeIds.Length > 0;
            var effectiveTypeIds = request.TypeId.HasValue ? [request.TypeId.Value] : savedTypeIds;
            var languageIds = preference.PreferenceLanguages.Select(link => link.LanguageId).Distinct().ToArray();
            var candidates = await _experienceRepository.GetRecommendationCandidatesAsync(cancellationToken);

            var requiredLanguages = preference.PreferenceLanguages
                .Where(link => link.Language is not null)
                .Select(link => link.Language.Name)
                .OrderBy(name => name)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var items = candidates
                .Where(candidate => candidate.Status == ExperienceStatus.Published)
                .Where(candidate => languageIds.Length == 0 || candidate.LanguageIds.Intersect(languageIds).Any())
                .Where(candidate => effectiveTypeIds.Length == 0 || effectiveTypeIds.Contains(candidate.ExperienceTypeId))
                .Select(candidate => Score(candidate, preference))
                .OrderByDescending(item => item.MatchPercentage)
                .ThenByDescending(item => item.AverageRating)
                .ThenBy(item => item.Id)
                .ToArray();

            return new RecommendedExperiencesResponse(
                items, types, request.TypeId, true, usesSavedTypes, requiredLanguages);
        }

        private static RecommendedExperienceDto Score(RecommendationExperienceCandidateDto experience, TouristPreference preference)
        {
            var score = 0m;
            var applicableWeight = 0m;
            var details = new List<string>();

            var preferredInterests = preference.PreferenceInterests.Select(link => link.InterestId).Distinct().ToArray();
            if (preferredInterests.Length > 0)
            {
                applicableWeight += InterestWeight;
                var matched = preferredInterests.Intersect(experience.InterestIds).Count();
                score += InterestWeight * matched / preferredInterests.Length;
                details.Add($"Interests: {matched} of {preferredInterests.Length} matched");
            }

            applicableWeight += BudgetWeight;
            var withinBudget = IsWithinBudget(experience.PricePerPerson, preference.Budget);
            if (withinBudget) score += BudgetWeight;
            details.Add(withinBudget ? "Within your budget" : "Outside your selected budget");

            applicableWeight += DurationWeight;
            var durationMatches = IsWithinPreferredDuration(experience.DurationMinutes, preference.Duration);
            if (durationMatches) score += DurationWeight;
            details.Add(durationMatches ? "Matches your preferred duration" : "Outside your preferred duration");

            var preferredAreas = preference.PreferenceCulturalAreas.Select(link => link.CulturalAreaId).Distinct().ToArray();
            if (preferredAreas.Length > 0)
            {
                applicableWeight += AreaWeight;
                var areaMatches = preferredAreas.Contains(experience.CulturalAreaId);
                if (areaMatches) score += AreaWeight;
                details.Add(areaMatches ? "Matches a preferred region" : "Outside your preferred regions");
            }

            if (experience.ReviewCount > 0)
            {
                applicableWeight += RatingWeight;
                score += RatingWeight * Math.Clamp(experience.AverageRating / 5m, 0m, 1m);
                details.Add($"Guest rating: {experience.AverageRating:0.0}/5");
            }
            else
            {
                details.Add("No guest ratings yet; remaining factors are reweighted");
            }

            var matchPercentage = applicableWeight == 0
                ? 0m
                : decimal.Round(score / applicableWeight * 100m, 1, MidpointRounding.AwayFromZero);

            return new RecommendedExperienceDto(
                experience.Id,
                experience.Title,
                experience.ExperienceType,
                experience.CulturalArea,
                experience.HostName,
                experience.CoverImageKey,
                experience.CulturalStory,
                experience.PricePerPerson,
                experience.DurationMinutes,
                experience.AverageRating,
                experience.ReviewCount,
                matchPercentage,
                details);
        }

        private static bool IsWithinBudget(decimal price, Budget budget) => budget switch
        {
            Budget.Under500 => price < 500m,
            Budget.From500To800 => price >= 500m && price < 800m,
            Budget.From800To1500 => price >= 800m && price < 1500m,
            Budget.From1500To3000 => price >= 1500m && price < 3000m,
            Budget.Over3000 => price >= 3000m,
            _ => false
        };

        private static bool IsWithinPreferredDuration(int durationMinutes, DurationPreference duration) => duration switch
        {
            DurationPreference.Under2h => durationMinutes < 120,
            DurationPreference.From2To4h => durationMinutes >= 120 && durationMinutes < 240,
            DurationPreference.From4To6h => durationMinutes >= 240 && durationMinutes < 360,
            DurationPreference.FullDay => durationMinutes >= 360,
            _ => false
        };
    }
}
