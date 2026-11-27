using System.Security.Claims;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Booking
{
    [Authorize(Roles = "Tourist")]
    public sealed class GuideBookingsController : Controller
    {
        private readonly IMediator _mediator;
        public GuideBookingsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Index(int guideProfileId, CancellationToken cancellationToken)
        {
            if (guideProfileId <= 0) return NotFound();
            var page = await _mediator.Send(new GetGuideBookingPageQuery(guideProfileId), cancellationToken);
            if (page is null) return NotFound();
            SetPageContext("Book a local guide");
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(
            [Bind(Prefix = "Form")] GuideBookingInputDto form,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return await ReturnFormAsync(form, cancellationToken);

            try
            {
                var review = await _mediator.Send(new PreviewGuideBookingQuery(form), cancellationToken);
                SetPageContext("Review your guide booking");
                return View("Review", review);
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(exception);
                return await ReturnFormAsync(form, cancellationToken);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(GuideBookingInputDto form, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (!ModelState.IsValid) return await ReturnFormAsync(form, cancellationToken);

            try
            {
                var bookingId = await _mediator.Send(new CreateGuideBookingCommand(userId, form), cancellationToken);
                return RedirectToAction("Index", "MockPayments", new { bookingId });
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(exception);
                return await ReturnFormAsync(form, cancellationToken);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var response = await _mediator.Send(
                new GetTouristGuideBookingConfirmationQuery(userId, id), cancellationToken);
            if (response is null) return NotFound();
            SetPageContext("Guide booking request saved");
            return View(response);
        }

        private async Task<IActionResult> ReturnFormAsync(GuideBookingInputDto form, CancellationToken cancellationToken)
        {
            if (form.GuideProfileId <= 0) return BadRequest();
            var page = await _mediator.Send(new GetGuideBookingPageQuery(form.GuideProfileId), cancellationToken);
            if (page is null) return NotFound();
            SetPageContext("Book a local guide");
            return View("Index", page with { Form = form });
        }

        private void AddValidationErrors(ValidationException exception)
        {
            var errors = exception.Errors.ToArray();
            if (errors.Length == 0)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return;
            }
            foreach (var error in errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        private void SetPageContext(string title)
        {
            ViewData["Title"] = title;
            ViewData["TouristPage"] = "booking";
        }
    }
}
