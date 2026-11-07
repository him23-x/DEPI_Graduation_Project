using System.Security.Claims;
using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
<<<<<<< HEAD
=======
using DiscoverEgypt.Application.ViewModels.Disputes;
>>>>>>> f567d3a (modificaion)
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

<<<<<<< HEAD
namespace DiscoverEgypt.Presentation.Controllers.Disputes;

[Authorize(Roles = "Host")]
[Route("Host/Disputes")]
public sealed class DisputesController(IMediator mediator) : Controller
=======
namespace DiscoverEgypt.Presentation.Controllers.Disputes
>>>>>>> f567d3a (modificaion)
{
    [Authorize(Roles = "Host")]
    [Route("Host/Disputes")]
    public sealed class DisputesController : Controller
    {
        private readonly IMediator _mediator;

        public DisputesController(IMediator mediator)
        {
            _mediator = mediator;
        }

<<<<<<< HEAD
        ViewData["Title"] = "Host Disputes";
        ViewData["Section"] = "Disputes";
        return View(result);
    }
=======
        private const int PageSize = 10;
>>>>>>> f567d3a (modificaion)

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            page = Math.Clamp(page, 1, 1_000_000);

            var result = await _mediator.Send(new GetHostDisputesQuery(userId, page, PageSize), cancellationToken);
            if (page > result.PageCount)
                return RedirectToAction(nameof(Index), new { page = result.PageCount });

<<<<<<< HEAD
        ViewData["Title"] = "Dispute details";
        ViewData["Section"] = "Dispute details";
        return View(dispute);
=======
            ViewData["Title"] = "Host Disputes";
            ViewData["Section"] = "Disputes";
            return View(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (id <= 0) return NotFound();

            var dispute = await _mediator.Send(new GetHostDisputeDetailsQuery(userId, id), cancellationToken);
            if (dispute is null) return NotFound();

            ViewData["Title"] = "Dispute details";
            ViewData["Section"] = "Dispute details";
            return View(dispute);
        }
>>>>>>> f567d3a (modificaion)
    }
}

