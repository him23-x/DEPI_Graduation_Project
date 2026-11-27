using System.Security.Claims;
using DiscoverEgypt.Application.Features.Payments.Requests.Commands;
using DiscoverEgypt.Application.Features.Payments.Requests.Queries;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Booking
{
    [Authorize(Roles = "Tourist")]
    public sealed class MockPaymentsController : Controller
    {
        private readonly IMediator _mediator;

        public MockPaymentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Index(int bookingId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (bookingId <= 0) return NotFound();

            var payment = await _mediator.Send(new GetTouristMockPaymentQuery(userId, bookingId), cancellationToken);
            if (payment is null) return NotFound();
            if (payment.PaymentStatus == PaymentStatus.Paid || payment.BookingStatus != BookingStatus.Pending)
            {
                SetPageContext("Payment result");
                return View("Result", payment);
            }

            ViewData["PaymentNotice"] = TempData["PaymentNotice"] as string;
            SetPageContext("Checkout with Stripe");
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCheckoutSession(int bookingId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (bookingId <= 0) return NotFound();

            var baseSuccessUrl = Url.Action(nameof(Success), "MockPayments", new { bookingId }, Request.Scheme, Request.Host.Value);
            var cancelUrl = Url.Action(nameof(Cancel), "MockPayments", new { bookingId }, Request.Scheme, Request.Host.Value);
            if (string.IsNullOrWhiteSpace(baseSuccessUrl) || string.IsNullOrWhiteSpace(cancelUrl))
                return BadRequest();

            var separator = baseSuccessUrl.Contains('?', StringComparison.Ordinal) ? "&" : "?";
            var successUrl = $"{baseSuccessUrl}{separator}session_id={{CHECKOUT_SESSION_ID}}";

            try
            {
                var session = await _mediator.Send(
                    new CreateStripeCheckoutSessionCommand(userId, bookingId, successUrl, cancelUrl),
                    cancellationToken);

                return Redirect(session.CheckoutUrl);
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(exception);
                return await ReturnPaymentPageAsync(userId, bookingId, cancellationToken);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(
            int bookingId,
            [FromQuery(Name = "session_id")] string? sessionId,
            CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (bookingId <= 0) return NotFound();
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                ModelState.AddModelError(string.Empty, "Missing Stripe checkout session identifier.");
                return await ReturnPaymentPageAsync(userId, bookingId, cancellationToken);
            }

            try
            {
                await _mediator.Send(new CompleteStripeCheckoutCommand(userId, bookingId, sessionId), cancellationToken);
                return RedirectToAction(nameof(Result), new { bookingId });
            }
            catch (ValidationException exception)
            {
                AddValidationErrors(exception);
                return await ReturnPaymentPageAsync(userId, bookingId, cancellationToken);
            }
        }

        [HttpGet]
        public IActionResult Cancel(int bookingId)
        {
            if (bookingId <= 0) return NotFound();
            TempData["PaymentNotice"] = "Stripe checkout was cancelled. You can complete your payment below while the booking is still pending.";
            return RedirectToAction(nameof(Index), new { bookingId });
        }

        [HttpGet]
        public async Task<IActionResult> Result(int bookingId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (bookingId <= 0) return NotFound();

            var payment = await _mediator.Send(new GetTouristMockPaymentQuery(userId, bookingId), cancellationToken);
            if (payment is null) return NotFound();

            SetPageContext("Payment result");
            return View(payment);
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpPost("/webhooks/stripe")]
        public async Task<IActionResult> Webhook(CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync(cancellationToken);
            var signature = Request.Headers["Stripe-Signature"].ToString();
            if (string.IsNullOrWhiteSpace(payload) || string.IsNullOrWhiteSpace(signature))
                return BadRequest();

            try
            {
                await _mediator.Send(new HandleStripeWebhookCommand(payload, signature), cancellationToken);
                return Ok();
            }
            catch (ValidationException)
            {
                return BadRequest();
            }
        }

        private async Task<IActionResult> ReturnPaymentPageAsync(string userId, int bookingId, CancellationToken cancellationToken)
        {
            var payment = await _mediator.Send(new GetTouristMockPaymentQuery(userId, bookingId), cancellationToken);
            if (payment is null) return NotFound();
            if (payment.PaymentStatus == PaymentStatus.Paid || payment.BookingStatus != BookingStatus.Pending)
            {
                SetPageContext("Payment result");
                return View("Result", payment);
            }

            SetPageContext("Checkout with Stripe");
            return View("Index", payment);
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
