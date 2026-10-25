using System.Security.Claims;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace DiscoverEgypt.Presentation.Controllers.Booking;

[Authorize(Roles = "Host")]
public sealed class BookingsController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(HostBookingTab tab = HostBookingTab.Pending, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1";
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        if (!Enum.IsDefined(tab)) tab = HostBookingTab.Pending;

        var bookings = await mediator.Send(new GetHostBookingsQuery(userId, tab), cancellationToken);
        ViewData["Section"] = "Bookings";
        return View(new HostBookingsIndexDto
        {
            SelectedTab = tab,
            Bookings = bookings,
            Notice = TempData["BookingNotice"] as string,
            Error = TempData["BookingError"] as string
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        if (id <= 0) return NotFound();

        var booking = await mediator.Send(new GetHostBookingDetailsQuery(userId, id), cancellationToken);
        if (booking is null) return NotFound();
        ViewData["Section"] = "Booking details";
        return View(new HostBookingDetailsDto
        {
            Booking = booking,
            Notice = TempData["BookingNotice"] as string,
            Error = TempData["BookingError"] as string
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Accept(int id, CancellationToken cancellationToken) =>
        ExecuteActionAsync(id, (userId, bookingId) => new AcceptHostBookingCommand(userId, bookingId),
            "Booking accepted. The payout is scheduled after the 24-hour complaint window.", cancellationToken);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Reject(int id, CancellationToken cancellationToken) =>
        ExecuteActionAsync(id, (userId, bookingId) => new RejectHostBookingCommand(userId, bookingId),
            "Booking rejected. A refund for the service amount was recorded; the platform fee is not refundable.", cancellationToken, true);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Complete(int id, CancellationToken cancellationToken) =>
        ExecuteActionAsync(id, (userId, bookingId) => new CompleteHostBookingCommand(userId, bookingId),
            "Booking marked as completed. Payout eligibility remains tied to its scheduled time.", cancellationToken);

    private async Task<IActionResult> ExecuteActionAsync(
        int id,
        Func<string, int, IRequest<HostBookingActionResult>> createCommand,
        string successMessage,
        CancellationToken cancellationToken,
        bool returnToList = false)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        if (id <= 0) return NotFound();

        var result = await mediator.Send(createCommand(userId, id), cancellationToken);
        if (result == HostBookingActionResult.BookingUnavailable) return NotFound();
        if (result == HostBookingActionResult.Success)
            TempData["BookingNotice"] = successMessage;
        else
            TempData["BookingError"] = ActionError(result);

        if (result == HostBookingActionResult.Success && returnToList)
            return RedirectToAction(nameof(Index), new { tab = HostBookingTab.RejectedCancelled });

        return RedirectToAction(nameof(Details), new { id });
    }

    private static string ActionError(HostBookingActionResult result) => result switch
    {
        HostBookingActionResult.NotPending => "Only pending bookings can be accepted or rejected.",
        HostBookingActionResult.PaymentNotPaid => "This booking is not available until payment is confirmed.",
        HostBookingActionResult.ResponseWindowExpired => "The 12-hour response window has expired.",
        HostBookingActionResult.ProviderSuspended => "Your account is suspended and cannot accept new bookings.",
        HostBookingActionResult.Overlap => "You already have a confirmed experience booking during this time.",
        HostBookingActionResult.ConcurrencyUnavailable => "The booking could not be locked safely. Refresh and try again.",
        HostBookingActionResult.PayoutAlreadyExists => "A payout already exists for this booking, so it cannot be accepted or rejected again.",
        HostBookingActionResult.InvalidRefundAmount => "This booking does not have a refundable service amount.",
        HostBookingActionResult.NotConfirmed => "Only confirmed bookings can be marked completed.",
        HostBookingActionResult.NotEnded => "The booking cannot be marked completed before its scheduled end.",
        HostBookingActionResult.OpenDispute => "This booking cannot be marked completed while a dispute is open or under review.",
        _ => "The booking could not be updated. Refresh and try again."
    };
}
