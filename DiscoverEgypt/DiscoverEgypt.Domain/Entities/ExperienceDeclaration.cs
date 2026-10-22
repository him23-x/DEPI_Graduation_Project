using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class ExperienceDeclaration : BaseEntity
{
    public int ExperienceId { get; set; }
    public Experience Experience { get; set; } = null!;
    public string HostUserId { get; set; }
    public DeclarationType DeclarationType { get; set; }
    public string DeclarationText { get; set; } = string.Empty;
    public DateTime AcceptedAt { get; set; }
}

