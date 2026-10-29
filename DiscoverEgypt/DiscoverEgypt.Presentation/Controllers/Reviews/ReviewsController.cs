using System.Security.Claims;
using AutoMapper;
using DiscoverEgypt.Application.Features.Reviews.Requests.Queries;
using DiscoverEgypt.Presentation.ViewModels.Reviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Reviews;

[Authorize(Roles = "Host")]
[Route("Host/Reviews")]
public sealed class ReviewsController(IMediator mediator, IMapper mapper) : Controller
{
    private const int PageSize = 10;

    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        page = Math.Clamp(page, 1, 1_000_000);

        var result = await mediator.Send(new GetHostReviewsQuery(userId, page, PageSize), cancellationToken);
        if (page > result.PageCount)
            return RedirectToAction(nameof(Index), new { page = result.PageCount });

        ViewData["Title"] = "Host Reviews";
        ViewData["Section"] = "Reviews";
        return View(mapper.Map<HostReviewsIndexViewModel>(result));
    }
}
