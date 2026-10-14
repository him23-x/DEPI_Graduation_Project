using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Bookings;

public record PriceBreakdownDto(
    decimal ServiceAmount,
    decimal PlatformFeeRate,
    decimal PlatformFeeAmount,
    decimal TotalAmount);

public record BookingDto(
    Guid Id,
    string ExperienceTitle,
    DateTime StartAt,
    int NumberOfGuests,
    decimal ServiceAmount,
    decimal PlatformFeeRate,
    decimal PlatformFeeAmount,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt)
{
    public static BookingDto From(Booking b, string experienceTitle) => new(
        b.Id, experienceTitle, b.StartAt, b.NumberOfGuests,
        b.ServiceAmount, b.PlatformFeeRate, b.PlatformFeeAmount, b.TotalAmount,
        b.Status.ToString(), b.CreatedAt);
}
