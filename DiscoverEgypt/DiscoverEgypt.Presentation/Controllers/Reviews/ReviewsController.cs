using System.Security.Claims;
using DiscoverEgypt.Application.Features.Reviews.Requests.Queries;
<<<<<<< HEAD
=======
using DiscoverEgypt.Application.ViewModels.Reviews;
>>>>>>> f567d3a (modificaion)
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

<<<<<<< HEAD
namespace DiscoverEgypt.Presentation.Controllers.Reviews;

[Authorize(Roles = "Host")]
[Route("Host/Reviews")]
public sealed class ReviewsController(IMediator mediator) : Controller
=======
namespace DiscoverEgypt.Presentation.Controllers.Reviews
>>>>>>> f567d3a (modificaion)
{
    [Authorize(Roles = "Host")]
    [Route("Host/Reviews")]
    public sealed class ReviewsController : Controller
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

<<<<<<< HEAD
        ViewData["Title"] = "Host Reviews";
        ViewData["Section"] = "Reviews";
        return View(result);
=======
        private const int PageSize = 10;

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            page = Math.Clamp(page, 1, 1_000_000);

            var result = await _mediator.Send(new GetHostReviewsQuery(userId, page, PageSize), cancellationToken);
            if (page > result.PageCount)
                return RedirectToAction(nameof(Index), new { page = result.PageCount });

            ViewData["Title"] = "Host Reviews";
            ViewData["Section"] = "Reviews";
            return View(result);
        }
>>>>>>> f567d3a (modificaion)
    }
}

