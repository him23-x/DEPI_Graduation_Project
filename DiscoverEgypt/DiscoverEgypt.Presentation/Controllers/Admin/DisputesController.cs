using System.Security.Claims;
using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Application.Features.AdminDisputes.Requests.Commands;
using DiscoverEgypt.Application.Features.AdminDisputes.Requests.Queries;
using DiscoverEgypt.Application.Enums.Disputes;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Disputes")]
    public sealed class DisputesController : Controller
    {
        private readonly IMediator _mediator;

        public DisputesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var disputes = await _mediator.Send(new GetAdminDisputeQueueQuery(), cancellationToken);
            ViewData["Title"] = "Dispute review queue";
            return View("~/Views/Admin/Disputes/Index.cshtml", disputes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) return NotFound();
            var dispute = await _mediator.Send(new GetAdminDisputeDetailsQuery(id), cancellationToken);
            if (dispute is null) return NotFound();

            ViewData["Title"] = "Review dispute";
            return View("~/Views/Admin/Disputes/Details.cshtml", dispute);
        }

        [HttpPost("{id:int}/resolve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(
            int id,
            AdminDisputeResolutionDto form,
            CancellationToken cancellationToken)
        {
            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(adminUserId)) return Challenge();
            if (id <= 0) return NotFound();

            try
            {
                var result = await _mediator.Send(new ResolveAdminDisputeCommand(adminUserId, id, form), cancellationToken);
                if (result.Outcome == AdminDisputeResolutionOutcome.DisputeUnavailable) return NotFound();
                if (result.Outcome == AdminDisputeResolutionOutcome.DisputeAlreadyResolved)
                {
                    TempData["AdminDisputeError"] = "This dispute has already been resolved.";
                    return Redirect("/Admin/Disputes");
                }

                TempData["AdminDisputeSuccess"] = "Dispute resolved. The refund, sanction, and payout records were updated according to the selected outcome.";
                return Redirect("/Admin/Disputes");
            }
            catch (ValidationException exception)
            {
                foreach (var error in exception.Errors)
                    ModelState.AddModelError(error.PropertyName.Split('.').Last(), error.ErrorMessage);

                ViewData["Outcome"] = form.Outcome?.ToString();
                ViewData["ResolutionNote"] = form.ResolutionNote;
                ViewData["SanctionDays"] = form.SanctionDays;
                return await ReturnDetailsAsync(id, cancellationToken);
            }
        }

        private async Task<IActionResult> ReturnDetailsAsync(int id, CancellationToken cancellationToken)
        {
            var dispute = await _mediator.Send(new GetAdminDisputeDetailsQuery(id), cancellationToken);
            if (dispute is null)
            {
                TempData["AdminDisputeError"] = "This dispute is no longer open for review.";
                return Redirect("/Admin/Disputes");
            }

            ViewData["Title"] = "Review dispute";
            return View("~/Views/Admin/Disputes/Details.cshtml", dispute);
        }
    }
}
