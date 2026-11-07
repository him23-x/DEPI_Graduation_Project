using DiscoverEgypt.Domain.Enums.Experiences;
using DiscoverEgypt.Application.ViewModels.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Policies
{
    public static class HostExperienceDraftPolicy
    {
        public static bool IsEditable(ExperienceStatus status) =>
            status is ExperienceStatus.Draft or ExperienceStatus.Rejected;

        public static bool HasValidOptions(HostExperienceDraftViewModel draft, ExperienceCreateOptionsViewModel options)
        {
            return options.ExperienceTypes.Any(item => item.Id == draft.ExperienceTypeId)
                && options.CulturalAreas.Any(item => item.Id == draft.CulturalAreaId)
                && draft.InterestIds.Count > 0
                && draft.InterestIds.All(id => options.Interests.Any(item => item.Id == id))
                && draft.LanguageIds.Count > 0
                && draft.LanguageIds.All(id => options.Languages.Any(item => item.Id == id));
        }

        public static HostExperienceDraftViewModel Normalize(HostExperienceDraftViewModel draft)
        {
            return new HostExperienceDraftViewModel
            {
                ExperienceTypeId = draft.ExperienceTypeId,
                CulturalAreaId = draft.CulturalAreaId,
                Title = draft.Title.Trim(),
                Description = draft.Description.Trim(),
                CulturalStory = draft.CulturalStory.Trim(),
                PricePerPerson = draft.PricePerPerson,
                DurationMinutes = draft.DurationMinutes,
                MaxGroupSize = draft.MaxGroupSize,
                MeetingPointArea = draft.MeetingPointArea.Trim(),
                ExactMeetingPoint = draft.ExactMeetingPoint.Trim(),
                InterestIds = draft.InterestIds.Distinct().ToArray(),
                LanguageIds = draft.LanguageIds.Distinct().ToArray()
            };
        }
    }
}
