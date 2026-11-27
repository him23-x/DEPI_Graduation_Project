using System.Security.Claims;
using DiscoverEgypt.Application.Features.Dashboard.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.TouristDashboard
{
    [Authorize(Roles = "Tourist")]
    public sealed class TouristDashboardController : Controller
    {
        private readonly IMediator _mediator;

        public TouristDashboardController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            var dashboard = await _mediator.Send(new GetTouristDashboardQuery(userId), cancellationToken);
            ViewData["Title"] = "Your travel dashboard";
            ViewData["TouristPage"] = "tourist-dashboard";
            return View(dashboard);
        }
    }
}
