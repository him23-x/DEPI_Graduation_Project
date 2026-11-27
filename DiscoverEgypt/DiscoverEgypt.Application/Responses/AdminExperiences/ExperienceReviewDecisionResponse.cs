using DiscoverEgypt.Application.Enums.AdminExperiences;

namespace DiscoverEgypt.Application.Responses.AdminExperiences
{
    public sealed record ExperienceReviewDecisionResponse(
        ExperienceReviewDecisionOutcome Outcome,
        int ExperienceId,
        IReadOnlyList<string> Issues);
}
