using DiscoverEgypt.Application.Bookings;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.WebApi.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly BookingService _service;

    public BookingsController(BookingService service)
    {
        _service = service;
    }

    // POST /api/bookings/preview : price breakdown only, nothing is saved
    [HttpPost("preview")]
    public async Task<ActionResult<PriceBreakdownDto>> Preview(CreateExperienceBookingRequest request)
    {
        return Ok(await _service.PreviewExperienceBookingAsync(request));
    }

    // POST /api/bookings : create the booking
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(CreateExperienceBookingRequest request)
    {
        return Ok(await _service.CreateExperienceBookingAsync(request));
    }

    // GET /api/bookings?touristId=...
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookingDto>>> GetMine([FromQuery] Guid touristId)
    {
        return Ok(await _service.GetMyBookingsAsync(touristId));
    }
}
