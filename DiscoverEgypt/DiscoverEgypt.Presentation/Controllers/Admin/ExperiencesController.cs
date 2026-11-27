using System.Security.Claims;
using DiscoverEgypt.Application.DTOs.AdminExperiences;
using DiscoverEgypt.Application.Enums.AdminExperiences;
using DiscoverEgypt.Application.Features.AdminExperiences.Requests.Commands;
using DiscoverEgypt.Application.Features.AdminExperiences.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Experiences")]
    public sealed class ExperiencesController : Controller
    {
        private readonly IMediator _mediator;

        public ExperiencesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var experiences = await _mediator.Send(new GetAdminExperienceQueueQuery(), cancellationToken);
            ViewData["Title"] = "Experience management";
            return View("~/Views/Admin/Experiences/Index.cshtml", experiences);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) return NotFound();

            var experience = await _mediator.Send(new GetAdminExperienceDetailsQuery(id), cancellationToken);
            if (experience is null) return NotFound();

            ViewData["Title"] = "Review experience";
            return View("~/Views/Admin/Experiences/Details.cshtml", experience);
        }

        [HttpPost("{id:int}/approve")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Approve(int id, CancellationToken cancellationToken) =>
            DecideAsync(id, ExperienceReviewDecision.Approve, new AdminExperienceDecisionDto(), cancellationToken);

        [HttpPost("{id:int}/reject")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Reject(
            int id,
            AdminExperienceDecisionDto form,
            CancellationToken cancellationToken) =>
            DecideAsync(id, ExperienceReviewDecision.Reject, form, cancellationToken);

        [HttpPost("{id:int}/unpublish")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Unpublish(int id, CancellationToken cancellationToken) =>
            DecideAsync(id, ExperienceReviewDecision.Unpublish, new AdminExperienceDecisionDto(), cancellationToken);

        [HttpPost("{id:int}/republish")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Republish(int id, CancellationToken cancellationToken) =>
            DecideAsync(id, ExperienceReviewDecision.Republish, new AdminExperienceDecisionDto(), cancellationToken);

        private async Task<IActionResult> DecideAsync(
            int id,
            ExperienceReviewDecision decision,
            AdminExperienceDecisionDto form,
            CancellationToken cancellationToken)
        {
            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(adminUserId)) return Challenge();
            if (id <= 0) return NotFound();

            try
            {
                var response = await _mediator.Send(
                    new DecideAdminExperienceReviewCommand(adminUserId, id, decision, form), cancellationToken);

                if (response.Outcome == ExperienceReviewDecisionOutcome.ExperienceUnavailable) return NotFound();
                if (response.Outcome == ExperienceReviewDecisionOutcome.NotPending)
                {
                    TempData["AdminExperienceReviewError"] = "This experience is no longer waiting for review.";
                    return RedirectToAction(nameof(Index));
                }
                if (response.Outcome is ExperienceReviewDecisionOutcome.NotPublished or ExperienceReviewDecisionOutcome.NotUnpublished)
                {
                    TempData["AdminExperienceReviewError"] = "The experience status changed. Refresh the list and try again.";
                    return RedirectToAction(nameof(Index));
                }
                if (response.Outcome == ExperienceReviewDecisionOutcome.RepublishPrerequisitesNotMet)
                {
                    TempData["AdminExperienceReviewError"] = "This experience cannot be republished yet: "
                        + string.Join(" ", response.Issues);
                    return await ReturnDetailsAsync(id, null, cancellationToken);
                }

                TempData["AdminExperienceReviewSuccess"] = decision switch
                {
                    ExperienceReviewDecision.Approve => "Experience approved and published.",
                    ExperienceReviewDecision.Reject => "Experience rejected and the reason was saved.",
                    ExperienceReviewDecision.Unpublish => "Experience unpublished.",
                    ExperienceReviewDecision.Republish => "Experience republished.",
                    _ => "Experience status updated."
                };
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException exception)
            {
                foreach (var error in exception.Errors)
                    ModelState.AddModelError(nameof(AdminExperienceDecisionDto.RejectionReason), error.ErrorMessage);

                return await ReturnDetailsAsync(id, form?.RejectionReason, cancellationToken);
            }
        }

        private async Task<IActionResult> ReturnDetailsAsync(
            int id,
            string? rejectionReason,
            CancellationToken cancellationToken)
        {
            var experience = await _mediator.Send(new GetAdminExperienceDetailsQuery(id), cancellationToken);
            if (experience is null)
            {
                TempData["AdminExperienceReviewError"] = "This experience is no longer waiting for review.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Title"] = "Review experience";
            ViewData["RejectionReason"] = rejectionReason;
            return View("~/Views/Admin/Experiences/Details.cshtml", experience);
        }
    }
}
