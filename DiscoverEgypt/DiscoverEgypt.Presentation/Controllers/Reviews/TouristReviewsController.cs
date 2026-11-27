using System.Security.Claims;
using DiscoverEgypt.Application.DTOs.Reviews;
using DiscoverEgypt.Application.Enums.Reviews;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Reviews.Requests.Commands;
using DiscoverEgypt.Application.Responses.Reviews;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Reviews
{
    [Authorize(Roles = "Tourist")]
    [Route("TouristReviews")]
    public sealed class TouristReviewsController : Controller
    {
        private readonly IMediator _mediator;

        public TouristReviewsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("create/{bookingId:int}")]
        public async Task<IActionResult> Create(int bookingId, CancellationToken cancellationToken)
        {
            var userId = CurrentUserId();
            if (userId is null) return Challenge();
            if (bookingId <= 0) return NotFound();

            var booking = await GetBookingAsync(userId, bookingId, cancellationToken);
            if (booking is null) return NotFound();
            if (!booking.CanReview)
            {
                TempData[booking.ReviewRating.HasValue ? "TouristReviewNotice" : "TouristReviewError"] = booking.ReviewRating.HasValue
                    ? "You have already reviewed this booking."
                    : "Only completed bookings can be reviewed.";
                return RedirectToAction("Details", "TouristBookings", new { id = bookingId });
            }

            SetPageContext();
            return View(new TouristReviewFormResponse(booking, new CreateTouristReviewDto { BookingId = bookingId }));
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(Prefix = "Form")] CreateTouristReviewDto form,
            CancellationToken cancellationToken)
        {
            var userId = CurrentUserId();
            if (userId is null) return Challenge();
            if (form.BookingId <= 0) return NotFound();

            if (!ModelState.IsValid)
                return await ReturnCreatePageAsync(userId, form, cancellationToken);

            try
            {
                var result = await _mediator.Send(new CreateTouristReviewCommand(userId, form), cancellationToken);
                if (result.Outcome == TouristReviewCreateOutcome.BookingUnavailable) return NotFound();
                if (result.Outcome == TouristReviewCreateOutcome.BookingNotCompleted)
                {
                    TempData["TouristReviewError"] = "Only completed bookings can be reviewed.";
                    return RedirectToAction("Details", "TouristBookings", new { id = form.BookingId });
                }
                if (result.Outcome == TouristReviewCreateOutcome.AlreadyReviewed)
                {
                    TempData["TouristReviewNotice"] = "You have already reviewed this booking.";
                    return RedirectToAction("Details", "TouristBookings", new { id = form.BookingId });
                }

                TempData["TouristReviewNotice"] = "Your review was submitted successfully.";
                return RedirectToAction("Details", "TouristBookings", new { id = result.BookingId });
            }
            catch (ValidationException exception)
            {
                foreach (var error in exception.Errors)
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                return await ReturnCreatePageAsync(userId, form, cancellationToken);
            }
        }

        private async Task<IActionResult> ReturnCreatePageAsync(
            string userId,
            CreateTouristReviewDto form,
            CancellationToken cancellationToken)
        {
            var booking = await GetBookingAsync(userId, form.BookingId, cancellationToken);
            if (booking is null) return NotFound();
            if (!booking.CanReview)
                return RedirectToAction("Details", "TouristBookings", new { id = form.BookingId });

            SetPageContext();
            return View("Create", new TouristReviewFormResponse(booking, form));
        }

        private Task<DiscoverEgypt.Application.Responses.Bookings.TouristBookingDetailsResponse?> GetBookingAsync(
            string userId,
            int bookingId,
            CancellationToken cancellationToken) =>
            _mediator.Send(new GetTouristBookingDetailsQuery(userId, bookingId), cancellationToken);

        private string? CurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private void SetPageContext()
        {
            ViewData["Title"] = "Review your booking";
            ViewData["TouristPage"] = "booking";
        }
    }
}
