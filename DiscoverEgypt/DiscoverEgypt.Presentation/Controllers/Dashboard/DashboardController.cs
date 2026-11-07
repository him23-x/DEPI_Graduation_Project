using System.Security.Claims;
using DiscoverEgypt.Application.Features.Dashboard.Requests.Queries;
<<<<<<< HEAD
=======
using DiscoverEgypt.Application.ViewModels.Dashboard;
>>>>>>> f567d3a (modificaion)
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

<<<<<<< HEAD
namespace DiscoverEgypt.Presentation.Controllers.Dashboard;

[Authorize(Roles = "Host")]
public sealed class DashboardController(IMediator mediator) : Controller
=======
namespace DiscoverEgypt.Presentation.Controllers.Dashboard
>>>>>>> f567d3a (modificaion)
{
    [Authorize(Roles = "Host")]
    public sealed class DashboardController : Controller
    {
        private readonly IMediator _mediator;

<<<<<<< HEAD
        var dashboard = await mediator.Send(new GetHostDashboardQuery(userId), cancellationToken);
        ViewData["Section"] = "Dashboard";
        return View(dashboard);
=======
        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            var dashboard = await _mediator.Send(new GetHostDashboardQuery(userId), cancellationToken);
            ViewData["Section"] = "Dashboard";
            return View(dashboard);
        }
>>>>>>> f567d3a (modificaion)
    }
}

