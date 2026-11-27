using DiscoverEgypt.Domain.Enums.Disputes;

namespace DiscoverEgypt.Application.DTOs.Disputes
{
    public sealed class AdminDisputeResolutionDto
    {
        public DisputeOutcome? Outcome { get; set; }
        public string? ResolutionNote { get; set; }
        public int? SanctionDays { get; set; }
    }
}
