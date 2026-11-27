using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Features.Explore.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Explore
{
    [AllowAnonymous]
    public sealed class ExploreController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IExperienceMediaStorage _mediaStorage;

        public ExploreController(IMediator mediator, IExperienceMediaStorage mediaStorage)
        {
            _mediator = mediator;
            _mediaStorage = mediaStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            int? typeId,
            int? areaId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sort,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            var results = await _mediator.Send(new ExploreExperiencesQuery(
                search, typeId, areaId, minPrice, maxPrice, sort, page), cancellationToken);
            ViewData["Title"] = "Explore authentic experiences";
            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> Image(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) return NotFound();
            var storageKey = await _mediator.Send(new GetPublishedExperienceImageKeyQuery(id), cancellationToken);
            if (string.IsNullOrWhiteSpace(storageKey)) return NotFound();

            try
            {
                var download = await _mediaStorage.OpenReadAsync(storageKey, cancellationToken);
                Response.Headers.CacheControl = "public, max-age=3600";
                return File(download.Content, download.ContentType);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
