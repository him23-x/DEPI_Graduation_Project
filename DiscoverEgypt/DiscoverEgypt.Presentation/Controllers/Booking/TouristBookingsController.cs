using System.Security.Claims;
using DiscoverEgypt.Application.Enums.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Booking
{
    [Authorize(Roles = "Tourist")]
    public sealed class TouristBookingsController : Controller
    {
        private readonly IMediator _mediator;
        public TouristBookingsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var bookings = await _mediator.Send(new GetTouristBookingsQuery(userId), cancellationToken);
            ViewData["Title"] = "My bookings";
            ViewData["TouristPage"] = "booking";
            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (id <= 0) return NotFound();
            var booking = await _mediator.Send(new GetTouristBookingDetailsQuery(userId, id), cancellationToken);
            if (booking is null) return NotFound();
            ViewData["Title"] = "Booking details";
            ViewData["TouristPage"] = "booking";
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (id <= 0) return NotFound();

            var outcome = await _mediator.Send(new CancelTouristBookingCommand(userId, id), cancellationToken);
            if (outcome == BookingCancellationOutcome.BookingUnavailable) return NotFound();
            if (outcome == BookingCancellationOutcome.Success)
                TempData["TouristBookingNotice"] = "Booking cancelled. The simulated refund and any provider allocation were recorded according to the cancellation policy.";
            else
                TempData["TouristBookingError"] = outcome == BookingCancellationOutcome.PayoutUnavailable
                    ? "The provider payout has already been processed, so this booking cannot be cancelled."
                    : "This booking is no longer eligible for cancellation.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
