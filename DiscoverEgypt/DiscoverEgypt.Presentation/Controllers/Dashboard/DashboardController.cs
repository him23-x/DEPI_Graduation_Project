using System.Security.Claims;
using AutoMapper;
using DiscoverEgypt.Application.Features.Dashboard.Requests.Queries;
using DiscoverEgypt.Presentation.ViewModels.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Dashboard;

[Authorize(Roles = "Host")]
public sealed class DashboardController(IMediator mediator, IMapper mapper) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();

        var dashboard = await mediator.Send(new GetHostDashboardQuery(userId), cancellationToken);
        ViewData["Section"] = "Dashboard";
        return View(mapper.Map<HostDashboardViewModel>(dashboard));
    }
}
