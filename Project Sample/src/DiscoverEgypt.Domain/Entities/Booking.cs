using DiscoverEgypt.Domain.Common;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

// A SUBSET of the Booking class in the Classes Specs (experience bookings only).
public class Booking
{
    public Guid Id { get; private set; }
    public BookingType BookingType { get; private set; }
    public Guid TouristId { get; private set; }
    public Guid? ExperienceId { get; private set; }
    public DateTime StartAt { get; private set; }
    public int NumberOfGuests { get; private set; }
    public decimal? ExperiencePricePerPerson { get; private set; }   // snapshot
    public decimal ServiceAmount { get; private set; }               // provider price
    public decimal PlatformFeeRate { get; private set; }             // snapshot
    public decimal PlatformFeeAmount { get; private set; }
    public decimal TotalAmount { get; private set; }                 // ServiceAmount + PlatformFeeAmount
    public BookingStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Booking() { }

    // The business rules live HERE, inside the Domain (not in a Service or a Controller).
    public static Booking CreateForExperience(
        Guid touristId, Experience experience, int numberOfGuests, DateTime startAt, decimal feeRate)
    {
        if (experience.Status != ExperienceStatus.Published)
            throw new DomainException("This experience is not published.");

        if (numberOfGuests < 1 || numberOfGuests > experience.MaxGroupSize)
            throw new DomainException($"Number of guests must be between 1 and {experience.MaxGroupSize}.");

        if (feeRate < 0 || feeRate > 1)
            throw new DomainException("Fee rate must be between 0 and 1.");

        var serviceAmount = experience.PricePerPerson * numberOfGuests;
        var feeAmount = Math.Round(serviceAmount * feeRate, 2, MidpointRounding.AwayFromZero);

        return new Booking
        {
            Id = Guid.NewGuid(),
            BookingType = BookingType.Experience,
            TouristId = touristId,
            ExperienceId = experience.Id,
            StartAt = startAt,
            NumberOfGuests = numberOfGuests,
            ExperiencePricePerPerson = experience.PricePerPerson,
            ServiceAmount = serviceAmount,
            PlatformFeeRate = feeRate,
            PlatformFeeAmount = feeAmount,
            TotalAmount = serviceAmount + feeAmount,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }
}
