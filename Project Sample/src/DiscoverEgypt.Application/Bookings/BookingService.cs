using DiscoverEgypt.Application.Abstractions;
using DiscoverEgypt.Application.Common;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Bookings;

public class BookingService
{
    private readonly IExperienceRepository _experiences;
    private readonly IBookingRepository _bookings;
    private readonly PlatformFeeSettings _fees;

    public BookingService(
        IExperienceRepository experiences,
        IBookingRepository bookings,
        PlatformFeeSettings fees)
    {
        _experiences = experiences;
        _bookings = bookings;
        _fees = fees;
    }

    // Use case 1: show the price breakdown. NOTHING is saved.
    public async Task<PriceBreakdownDto> PreviewExperienceBookingAsync(CreateExperienceBookingRequest request)
    {
        var (booking, _) = await BuildAsync(request);
        return new PriceBreakdownDto(
            booking.ServiceAmount, booking.PlatformFeeRate,
            booking.PlatformFeeAmount, booking.TotalAmount);
    }

    // Use case 2: confirm the booking and save it.
    public async Task<BookingDto> CreateExperienceBookingAsync(CreateExperienceBookingRequest request)
    {
        var (booking, experience) = await BuildAsync(request);
        await _bookings.AddAsync(booking);
        return BookingDto.From(booking, experience.Title);
    }

    // Use case 3: "my bookings"
    public async Task<IReadOnlyList<BookingDto>> GetMyBookingsAsync(Guid touristId)
    {
        var bookings = await _bookings.GetByTouristAsync(touristId);
        var result = new List<BookingDto>();
        foreach (var booking in bookings)
        {
            var experience = booking.ExperienceId is { } id
                ? await _experiences.GetByIdAsync(id)
                : null;
            result.Add(BookingDto.From(booking, experience?.Title ?? string.Empty));
        }
        return result;
    }

    // The Application only ORCHESTRATES: load data, call the Domain, save.
    // The price rules themselves are inside Booking.CreateForExperience (Domain).
    private async Task<(Booking Booking, Experience Experience)> BuildAsync(CreateExperienceBookingRequest request)
    {
        var experience = await _experiences.GetByIdAsync(request.ExperienceId)
            ?? throw new NotFoundException("Experience not found.");

        var booking = Booking.CreateForExperience(
            request.TouristId, experience, request.NumberOfGuests,
            request.StartAt, _fees.ExperienceRate);

        return (booking, experience);
    }
}
