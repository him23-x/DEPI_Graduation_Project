using DiscoverEgypt.Domain.Enums.Hosts;

namespace DiscoverEgypt.Application.DTOs.HostProfile
{
    public sealed class HostProfileDto
    {
        public HostType HostType { get; set; }
        public string Bio { get; set; } = string.Empty;
        public bool HasProfileImage { get; set; }
    }
}
