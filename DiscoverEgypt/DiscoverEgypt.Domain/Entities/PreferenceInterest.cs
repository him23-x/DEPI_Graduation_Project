using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class PreferenceInterest : BaseEntity
{
    public int TouristPreferenceId { get; set; }
    public TouristPreference TouristPreference { get; set; } = null!;
    public int InterestId { get; set; }
    public Interest Interest { get; set; } = null!;
}

