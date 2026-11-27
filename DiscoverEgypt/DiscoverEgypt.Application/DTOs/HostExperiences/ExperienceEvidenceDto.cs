
using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences
{
    public sealed class ExperienceEvidenceDto
    {
        public required ExperienceEvidenceEditorDto Editor { get; set; }
        public string? TextContent { get; set; }
        public ExperienceVenueUse VenueUse { get; set; } = ExperienceVenueUse.PublicRoute;
        public bool EvidenceInHostName { get; set; } = true;
        public EvidenceKind FileKind { get; set; }
    }
}

