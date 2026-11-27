using System.Security.Claims;
using DiscoverEgypt.Application.Enums.BookingShares;
using DiscoverEgypt.Application.Features.BookingShares.Requests.Commands;
using DiscoverEgypt.Application.Features.BookingShares.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DiscoverEgypt.Presentation.Formatting;

namespace DiscoverEgypt.Presentation.Controllers.BookingShares
{
    [Route("booking-shares")]
    public sealed class BookingSharesController : Controller
    {
        private readonly IMediator _mediator;

        public BookingSharesController(IMediator mediator) => _mediator = mediator;

        [Authorize(Roles = "Tourist")]
        [HttpPost("create/{bookingId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bookingId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (bookingId <= 0) return NotFound();

            try
            {
                var response = await _mediator.Send(
                    new CreateTouristBookingShareCommand(userId, bookingId), cancellationToken);
                if (response.Outcome == BookingShareCreateOutcome.BookingUnavailable) return NotFound();
                if (response.Outcome == BookingShareCreateOutcome.BookingNotEligible)
                {
                    TempData["TouristBookingShareError"] = "A share link is available only for a confirmed, paid booking before its scheduled end.";
                    return RedirectToAction("Details", "TouristBookings", new { id = bookingId });
                }

                var shareUrl = Url.Action(nameof(Shared), "BookingShares", new { token = response.Token }, Request.Scheme);
                if (string.IsNullOrWhiteSpace(shareUrl))
                    throw new InvalidOperationException("The booking-share link could not be generated.");

                TempData["TouristBookingShareUrl"] = shareUrl;
                TempData["TouristBookingShareNotice"] = response.ExpiresAtUtc.HasValue
                    ? $"Your read-only link is ready. It expires at the end of the booking: {HostDateTimeFormatter.Cairo(response.ExpiresAtUtc.Value, "MMM d, yyyy · h:mm tt").Trim()}."
                    : "Your read-only link is ready.";
            }
            catch (ValidationException exception)
            {
                TempData["TouristBookingShareError"] = exception.Errors.FirstOrDefault()?.ErrorMessage
                    ?? "The booking-share link could not be created.";
            }

            return RedirectToAction("Details", "TouristBookings", new { id = bookingId });
        }

        [AllowAnonymous]
        [HttpGet("{token}")]
        public async Task<IActionResult> Shared(string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token) || token.Length > 256) return NotFound();

            var booking = await _mediator.Send(new GetSharedBookingQuery(token), cancellationToken);
            if (booking is null) return NotFound();

            ViewData["Title"] = "Shared booking details";
            ViewData["TouristPage"] = "shared-booking";
            return View("Shared", booking);
        }
    }
}
