using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/ExperienceEvidence")]
    public sealed class ExperienceEvidenceController : Controller
    {
        private readonly IMediator _mediator;

        public ExperienceEvidenceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{evidenceId:int}")]
        public async Task<IActionResult> Download(int evidenceId, CancellationToken cancellationToken)
        {
            var download = await _mediator.Send(
                new GetHostExperienceEvidenceFileForReviewQuery(evidenceId), cancellationToken);
            if (download is null)
                return NotFound();

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return File(download.Content, "application/octet-stream", $"experience-evidence-{evidenceId}");
        }
    }
}

