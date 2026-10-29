using DiscoverEgypt.Application.DTOs.HostExperiences;

using DiscoverEgypt.Application.Enums.HostExperiences;

namespace DiscoverEgypt.Presentation.ViewModels.Experience;

public sealed class SubmitExperienceViewModel
{
    public int ExperienceId { get; set; }
    public bool Accepted { get; set; }
    public ExperienceVenueUse? VenueUse { get; set; }
    public bool EvidenceInHostName { get; set; } = true;
}
