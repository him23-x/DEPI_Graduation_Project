using DiscoverEgypt.Application.Enums.GuideVerification;

namespace DiscoverEgypt.Application.Responses.GuideVerification
{
    public sealed record AdminGuideVerificationDecisionResponse(
        GuideVerificationDecisionOutcome Outcome,
        int VerificationRequestId);
}
