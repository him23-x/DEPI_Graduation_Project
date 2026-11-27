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
    public sealed class ExperienceBookingsController : Controller
    {
        private readonly IMediator _mediator;

        public ExperienceBookingsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Index(int experienceId, CancellationToken cancellationToken)
        {
            if (experienceId <= 0) return NotFound();
            var page = await _mediator.Send(new GetExperienceBookingPageQuery(experienceId), cancellationToken);
            if (page is null) return NotFound();

            SetPageContext("Book an experience");
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(
            [Bind(Prefix = "Form")] ExperienceBookingInputDto form,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return await ReturnFormAsync(form, cancellationToken);

            try
            {
                var review = await _mediator.Send(new PreviewExperienceBookingQuery(form), cancellationToken);
                SetPageContext("Review your booking");
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
        public async Task<IActionResult> Confirm(ExperienceBookingInputDto form, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            if (!ModelState.IsValid)
                return await ReturnFormAsync(form, cancellationToken);

            try
            {
                var bookingId = await _mediator.Send(new CreateExperienceBookingCommand(userId, form), cancellationToken);
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

            var confirmation = await _mediator.Send(
                new GetTouristExperienceBookingConfirmationQuery(userId, id), cancellationToken);
            if (confirmation is null) return NotFound();

            SetPageContext("Booking request saved");
            return View(confirmation);
        }

        private async Task<IActionResult> ReturnFormAsync(ExperienceBookingInputDto form, CancellationToken cancellationToken)
        {
            if (form.ExperienceId <= 0) return BadRequest();
            var page = await _mediator.Send(new GetExperienceBookingPageQuery(form.ExperienceId), cancellationToken);
            if (page is null) return NotFound();

            SetPageContext("Book an experience");
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
