using System.Security.Claims;
using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Application.Enums.Disputes;
using DiscoverEgypt.Application.Features.Disputes.Requests.Commands;
using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using DiscoverEgypt.Application.Responses.Disputes;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Disputes
{
    [Authorize(Roles = "Tourist")]
    [Route("TouristDisputes")]
    public sealed class TouristDisputesController : Controller
    {
        private readonly IMediator _mediator;

        public TouristDisputesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userId = CurrentUserId();
            if (userId is null) return Challenge();

            var disputes = await _mediator.Send(new GetTouristDisputesQuery(userId), cancellationToken);
            ViewData["Title"] = "My disputes";
            ViewData["TouristPage"] = "disputes";
            return View(disputes);
        }

        [HttpGet("open/{bookingId:int}")]
        public async Task<IActionResult> Open(int bookingId, CancellationToken cancellationToken)
        {
            var userId = CurrentUserId();
            if (userId is null) return Challenge();
            if (bookingId <= 0) return NotFound();

            var booking = await _mediator.Send(new GetTouristDisputeBookingQuery(userId, bookingId), cancellationToken);
            if (booking is null) return NotFound();

            SetOpenPageContext();
            return View(new TouristDisputeFormResponse(booking, new OpenTouristDisputeDto { BookingId = bookingId }));
        }

        [HttpPost("open")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Open(
            [Bind(Prefix = "Form")] OpenTouristDisputeDto form,
            CancellationToken cancellationToken)
        {
            var userId = CurrentUserId();
            if (userId is null) return Challenge();

            if (!ModelState.IsValid)
                return await ReturnOpenPageAsync(userId, form, cancellationToken);

            try
            {
                var result = await _mediator.Send(new OpenTouristDisputeCommand(userId, form), cancellationToken);
                if (result.Outcome == DisputeOpenOutcome.BookingUnavailable) return NotFound();
                if (result.Outcome == DisputeOpenOutcome.BookingNotEligible)
                {
                    TempData["TouristDisputeError"] = "This booking is no longer eligible for a dispute.";
                    return RedirectToAction("Details", "TouristBookings", new { id = form.BookingId });
                }

                TempData["TouristDisputeNotice"] = "Your dispute was submitted and is now open for review.";
                return RedirectToAction(nameof(Details), new { id = result.DisputeId });
            }
            catch (ValidationException exception)
            {
                foreach (var error in exception.Errors)
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                return await ReturnOpenPageAsync(userId, form, cancellationToken);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var userId = CurrentUserId();
            if (userId is null) return Challenge();
            if (id <= 0) return NotFound();

            var dispute = await _mediator.Send(new GetTouristDisputeDetailsQuery(userId, id), cancellationToken);
            if (dispute is null) return NotFound();

            ViewData["Title"] = "Dispute details";
            ViewData["TouristPage"] = "disputes";
            return View(dispute);
        }

        private async Task<IActionResult> ReturnOpenPageAsync(
            string userId,
            OpenTouristDisputeDto form,
            CancellationToken cancellationToken)
        {
            var booking = await _mediator.Send(
                new GetTouristDisputeBookingQuery(userId, form.BookingId), cancellationToken);
            if (booking is null) return NotFound();
            SetOpenPageContext();
            return View("Open", new TouristDisputeFormResponse(booking, form));
        }

        private string? CurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private void SetOpenPageContext()
        {
            ViewData["Title"] = "Open a dispute";
            ViewData["TouristPage"] = "disputes";
        }
    }
}
