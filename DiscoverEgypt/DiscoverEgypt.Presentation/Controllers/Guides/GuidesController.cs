using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Features.ApprovedGuides.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Guides
{
    [AllowAnonymous]
    public sealed class GuidesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IExperienceMediaStorage _mediaStorage;

        public GuidesController(IMediator mediator, IExperienceMediaStorage mediaStorage)
        {
            _mediator = mediator;
            _mediaStorage = mediaStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search, int? languageId, int? culturalAreaId, int page = 1,
            CancellationToken cancellationToken = default)
        {
            var guides = await _mediator.Send(
                new GetApprovedGuidesQuery(search, languageId, culturalAreaId, page), cancellationToken);
            ViewData["Title"] = "Approved Local Guides";
            ViewData["TouristPage"] = "guides";
            return View(guides);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) return NotFound();
            var guide = await _mediator.Send(new GetApprovedGuideDetailsQuery(id), cancellationToken);
            if (guide is null) return NotFound();

            ViewData["Title"] = guide.Profile.FullName;
            ViewData["TouristPage"] = "guide-details";
            return View(guide);
        }

        [HttpGet]
        public async Task<IActionResult> Image(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) return NotFound();
            var key = await _mediator.Send(new GetApprovedGuideImageKeyQuery(id), cancellationToken);
            if (string.IsNullOrWhiteSpace(key)) return NotFound();

            try
            {
                var image = await _mediaStorage.OpenReadAsync(key, cancellationToken);
                Response.Headers.CacheControl = "public, max-age=3600";
                return File(image.Content, image.ContentType);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
