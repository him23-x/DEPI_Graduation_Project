
using DiscoverEgypt.Domain.Enums.Sanctions;

namespace DiscoverEgypt.Domain.Entities;

public class Sanction : BaseEntity
{
    public string UserId { get; set; }
    public int DisputeId { get; set; }
    public Dispute Dispute { get; set; } = null!;
    public SanctionType SanctionType { get; set; }
    public int Days { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string DecidedByAdminId { get; set; }
}

