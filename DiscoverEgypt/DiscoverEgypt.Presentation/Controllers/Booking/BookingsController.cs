using System.Security.Claims;
using AutoMapper;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Commands;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Presentation.ViewModels.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

using DiscoverEgypt.Application.Enums.Bookings;

namespace DiscoverEgypt.Presentation.Controllers.Booking;

[Authorize(Roles = "Host")]
public sealed class BookingsController(IMediator mediator, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(HostBookingTab tab = HostBookingTab.Pending, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1";
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        if (!Enum.IsDefined(tab)) tab = HostBookingTab.Pending;

        var bookings = await mediator.Send(new GetHostBookingsQuery(userId, tab), cancellationToken);
        ViewData["Section"] = "Bookings";
        var model = new HostBookingsIndexDto
        {
            SelectedTab = tab,
            Bookings = bookings,
            Notice = TempData["BookingNotice"] as string,
            Error = TempData["BookingError"] as string
        };
        return View(mapper.Map<HostBookingsIndexViewModel>(model));
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
        var model = new HostBookingDetailsDto
        {
            Booking = booking,
            Notice = TempData["BookingNotice"] as string,
            Error = TempData["BookingError"] as string
        };
        return View(mapper.Map<HostBookingDetailsViewModel>(model));
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
        Func<string, int, IRequest<HostBookingActionOutcome>> createCommand,
        string successMessage,
        CancellationToken cancellationToken,
        bool returnToList = false)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        if (id <= 0) return NotFound();

        var result = await mediator.Send(createCommand(userId, id), cancellationToken);
        if (result == HostBookingActionOutcome.BookingUnavailable) return NotFound();
        if (result == HostBookingActionOutcome.Success)
            TempData["BookingNotice"] = successMessage;
        else
            TempData["BookingError"] = ActionError(result);

        if (result == HostBookingActionOutcome.Success && returnToList)
            return RedirectToAction(nameof(Index), new { tab = HostBookingTab.RejectedCancelled });

        return RedirectToAction(nameof(Details), new { id });
    }

    private static string ActionError(HostBookingActionOutcome result) => result switch
    {
        HostBookingActionOutcome.NotPending => "Only pending bookings can be accepted or rejected.",
        HostBookingActionOutcome.PaymentNotPaid => "This booking is not available until payment is confirmed.",
        HostBookingActionOutcome.ResponseWindowExpired => "The 12-hour response window has expired.",
        HostBookingActionOutcome.ProviderSuspended => "Your account is suspended and cannot accept new bookings.",
        HostBookingActionOutcome.Overlap => "You already have a confirmed experience booking during this time.",
        HostBookingActionOutcome.ConcurrencyUnavailable => "The booking could not be locked safely. Refresh and try again.",
        HostBookingActionOutcome.PayoutAlreadyExists => "A payout already exists for this booking, so it cannot be accepted or rejected again.",
        HostBookingActionOutcome.InvalidRefundAmount => "This booking does not have a refundable service amount.",
        HostBookingActionOutcome.NotConfirmed => "Only confirmed bookings can be marked completed.",
        HostBookingActionOutcome.NotEnded => "The booking cannot be marked completed before its scheduled end.",
        HostBookingActionOutcome.OpenDispute => "This booking cannot be marked completed while a dispute is open or under review.",
        _ => "The booking could not be updated. Refresh and try again."
    };
}
