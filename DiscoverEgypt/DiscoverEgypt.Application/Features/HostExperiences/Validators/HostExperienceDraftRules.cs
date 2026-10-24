using DiscoverEgypt.Application.DTOs.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators;

internal static class HostExperienceDraftRules
{
    public static bool IsValid(CreateHostExperienceDraftRequest request)
    {
        return !(request.ExperienceTypeId <= 0
            || request.CulturalAreaId <= 0
            || string.IsNullOrWhiteSpace(request.Title)
            || request.Title.Trim().Length > 200
            || string.IsNullOrWhiteSpace(request.Description)
            || request.Description.Trim().Length > 5000
            || string.IsNullOrWhiteSpace(request.CulturalStory)
            || request.CulturalStory.Trim().Length > 5000
            || request.PricePerPerson <= 0
            || request.PricePerPerson > 9999999999999999.99m
            || request.DurationMinutes <= 0
            || request.MaxGroupSize <= 0
            || string.IsNullOrWhiteSpace(request.MeetingPointArea)
            || request.MeetingPointArea.Trim().Length > 300
            || string.IsNullOrWhiteSpace(request.ExactMeetingPoint)
            || request.ExactMeetingPoint.Trim().Length > 1000
            || request.InterestIds.Count == 0
            || request.LanguageIds.Count == 0);
    }
}
