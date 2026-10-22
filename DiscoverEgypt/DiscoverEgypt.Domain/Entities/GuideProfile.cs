using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class GuideProfile : BaseEntity
{
    public string UserId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public string? ProfileImageKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<VerificationRequest> VerificationRequests { get; set; } = new List<VerificationRequest>();
    public ICollection<GuideLanguage> GuideLanguages { get; set; } = new List<GuideLanguage>();
    public ICollection<GuideCulturalArea> GuideCulturalAreas { get; set; } = new List<GuideCulturalArea>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

