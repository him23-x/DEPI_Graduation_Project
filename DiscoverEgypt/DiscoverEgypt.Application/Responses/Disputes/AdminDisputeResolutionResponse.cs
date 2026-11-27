using DiscoverEgypt.Application.Enums.Disputes;

namespace DiscoverEgypt.Application.Responses.Disputes
{
    public sealed record AdminDisputeResolutionResponse(
        AdminDisputeResolutionOutcome Outcome,
        int DisputeId);
}
