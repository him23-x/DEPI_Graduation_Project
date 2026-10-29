using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.Enums.HostExperiences;

public enum ExperienceMediaMutationOutcome
{
    Success, ExperienceUnavailable, ImageLimitReached, VideoLimitReached,
    InvalidFile, MediaUnavailable, CoverMustBeImage, InvalidOrder
}
