using System.Security.Claims;
using DiscoverEgypt.Application.Features.Dashboard.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Dashboard;

[Authorize(Roles = "Host")]
public sealed class DashboardController(IMediator mediator) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();

        var dashboard = await mediator.Send(new GetHostDashboardQuery(userId), cancellationToken);
        ViewData["Section"] = "Dashboard";
        return View(dashboard);
    }
}
