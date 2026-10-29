using System.Security.Claims;
using AutoMapper;
using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using DiscoverEgypt.Presentation.ViewModels.Disputes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Disputes;

[Authorize(Roles = "Host")]
[Route("Host/Disputes")]
public sealed class DisputesController(IMediator mediator, IMapper mapper) : Controller
{
    private const int PageSize = 10;

    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        page = Math.Clamp(page, 1, 1_000_000);

        var result = await mediator.Send(new GetHostDisputesQuery(userId, page, PageSize), cancellationToken);
        if (page > result.PageCount)
            return RedirectToAction(nameof(Index), new { page = result.PageCount });

        ViewData["Title"] = "Host Disputes";
        ViewData["Section"] = "Disputes";
        return View(mapper.Map<HostDisputesIndexViewModel>(result));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        if (id <= 0) return NotFound();

        var dispute = await mediator.Send(new GetHostDisputeDetailsQuery(userId, id), cancellationToken);
        if (dispute is null) return NotFound();

        ViewData["Title"] = "Dispute details";
        ViewData["Section"] = "Dispute details";
        return View(mapper.Map<HostDisputeDetailsViewModel>(dispute));
    }
}
