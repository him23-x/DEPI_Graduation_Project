using DiscoverEgypt.Domain.Common;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

// A SUBSET of the Experience class in the Classes Specs (enough for this sample).
public class Experience
{
    public Guid Id { get; private set; }
    public Guid HostProfileId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal PricePerPerson { get; private set; }
    public int DurationMinutes { get; private set; }
    public int MaxGroupSize { get; private set; }
    public string MeetingPoint { get; private set; } = string.Empty;
    public ExperienceStatus Status { get; private set; }

    private Experience() { }

    public Experience(Guid id, Guid hostProfileId, string title, string description,
        decimal pricePerPerson, int durationMinutes, int maxGroupSize,
        string meetingPoint, ExperienceStatus status)
    {
        if (pricePerPerson <= 0) throw new DomainException("PricePerPerson must be greater than 0.");
        if (durationMinutes <= 0) throw new DomainException("DurationMinutes must be greater than 0.");
        if (maxGroupSize < 1) throw new DomainException("MaxGroupSize must be at least 1.");

        Id = id;
        HostProfileId = hostProfileId;
        Title = title;
        Description = description;
        PricePerPerson = pricePerPerson;
        DurationMinutes = durationMinutes;
        MaxGroupSize = maxGroupSize;
        MeetingPoint = meetingPoint;
        Status = status;
    }
}
