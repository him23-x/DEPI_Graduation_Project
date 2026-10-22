using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class Experience : BaseEntity
{
    public int HostProfileId { get; set; }
    public HostProfile HostProfile { get; set; } = null!;
    public int ExperienceTypeId { get; set; }
    public ExperienceType ExperienceType { get; set; } = null!;
    public int CulturalAreaId { get; set; }
    public CulturalArea CulturalArea { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CulturalStory { get; set; } = string.Empty;
    public decimal PricePerPerson { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxGroupSize { get; set; }
    public string MeetingPointArea { get; set; } = string.Empty;
    public string ExactMeetingPoint { get; set; } = string.Empty;
    public ExperienceStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? ReviewedByAdminId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ExperienceMedia> Media { get; set; } = new List<ExperienceMedia>();
    public ICollection<ExperienceEvidence> Evidence { get; set; } = new List<ExperienceEvidence>();
    public ICollection<ExperienceDeclaration> Declarations { get; set; } = new List<ExperienceDeclaration>();
    public ICollection<ExperienceInterest> ExperienceInterests { get; set; } = new List<ExperienceInterest>();
    public ICollection<ExperienceLanguage> ExperienceLanguages { get; set; } = new List<ExperienceLanguage>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

