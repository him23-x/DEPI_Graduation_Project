using DiscoverEgypt.Application.Enums.Disputes;

namespace DiscoverEgypt.Application.Responses.Disputes
{
    public sealed record TouristDisputeOpenResponse(DisputeOpenOutcome Outcome, int? DisputeId);
}
