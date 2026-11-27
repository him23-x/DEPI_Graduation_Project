using System.Security.Claims;
using DiscoverEgypt.Application.DTOs.GuideVerification;
using DiscoverEgypt.Application.Enums.GuideVerification;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Commands;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/GuideVerifications")]
    public sealed class GuideVerificationsController : Controller
    {
        private readonly IMediator _mediator;

        public GuideVerificationsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var requests = await _mediator.Send(new GetAdminGuideVerificationQueueQuery(), cancellationToken);
            ViewData["Title"] = "Guide verification queue";
            return View("~/Views/Admin/GuideVerification/Index.cshtml", requests);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) return NotFound();

            var request = await _mediator.Send(new GetAdminGuideVerificationDetailsQuery(id), cancellationToken);
            if (request is null) return NotFound();

            ViewData["Title"] = "Review guide verification";
            return View("~/Views/Admin/GuideVerification/Details.cshtml", request);
        }

        [HttpGet("documents/{documentId:int}")]
        public async Task<IActionResult> DownloadDocument(int documentId, CancellationToken cancellationToken)
        {
            if (documentId <= 0) return NotFound();
            var download = await _mediator.Send(
                new GetAdminGuideVerificationDocumentQuery(documentId), cancellationToken);
            if (download is null) return NotFound();

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return File(download.Content, "application/octet-stream", $"guide-verification-document-{documentId}");
        }

        [HttpPost("{id:int}/approve")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Approve(int id, CancellationToken cancellationToken) =>
            DecideAsync(id, GuideVerificationDecision.Approve, new AdminGuideVerificationDecisionDto(), cancellationToken);

        [HttpPost("{id:int}/reject")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Reject(
            int id,
            AdminGuideVerificationDecisionDto form,
            CancellationToken cancellationToken) =>
            DecideAsync(id, GuideVerificationDecision.Reject, form, cancellationToken);

        private async Task<IActionResult> DecideAsync(
            int id,
            GuideVerificationDecision decision,
            AdminGuideVerificationDecisionDto form,
            CancellationToken cancellationToken)
        {
            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(adminUserId)) return Challenge();
            if (id <= 0) return NotFound();

            try
            {
                var response = await _mediator.Send(new DecideAdminGuideVerificationCommand(
                    adminUserId, id, decision, form), cancellationToken);

                if (response.Outcome == GuideVerificationDecisionOutcome.RequestUnavailable) return NotFound();
                if (response.Outcome == GuideVerificationDecisionOutcome.NotPending)
                {
                    TempData["AdminGuideVerificationError"] = "This verification request is no longer pending.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["AdminGuideVerificationSuccess"] = decision == GuideVerificationDecision.Approve
                    ? "Guide verification approved."
                    : "Guide verification rejected and the reason was saved.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException exception)
            {
                foreach (var error in exception.Errors)
                    ModelState.AddModelError(nameof(AdminGuideVerificationDecisionDto.RejectionReason), error.ErrorMessage);
                return await ReturnDetailsAsync(id, form?.RejectionReason, cancellationToken);
            }
        }

        private async Task<IActionResult> ReturnDetailsAsync(
            int id,
            string? rejectionReason,
            CancellationToken cancellationToken)
        {
            var request = await _mediator.Send(new GetAdminGuideVerificationDetailsQuery(id), cancellationToken);
            if (request is null)
            {
                TempData["AdminGuideVerificationError"] = "This verification request is no longer pending.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Title"] = "Review guide verification";
            ViewData["RejectionReason"] = rejectionReason;
            return View("~/Views/Admin/GuideVerification/Details.cshtml", request);
        }
    }
}
