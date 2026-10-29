
using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Application.DTOs.HostExperiences;

public sealed class SubmitExperienceDto
{
    public int ExperienceId { get; set; }
    public bool Accepted { get; set; }
    public ExperienceVenueUse? VenueUse { get; set; }
    public bool EvidenceInHostName { get; set; } = true;
}

