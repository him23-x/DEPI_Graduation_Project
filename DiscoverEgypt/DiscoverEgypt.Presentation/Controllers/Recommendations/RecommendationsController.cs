using System.Security.Claims;
using DiscoverEgypt.Application.Features.Recommendations.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Recommendations
{
    [Authorize(Roles = "Tourist")]
    public sealed class RecommendationsController : Controller
    {
        private readonly IMediator _mediator;

        public RecommendationsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Index(int? typeId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            var response = await _mediator.Send(
                new GetRecommendedExperiencesQuery(userId, typeId), cancellationToken);
            ViewData["Title"] = "Experiences picked for you";
            ViewData["TouristPage"] = "recommendations";
            return View(response);
        }
    }
}
