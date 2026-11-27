using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Features.ExperienceDetails.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.ExperienceDetails
{
    [AllowAnonymous]
    public sealed class ExperienceDetailsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IExperienceMediaStorage _mediaStorage;

        public ExperienceDetailsController(IMediator mediator, IExperienceMediaStorage mediaStorage)
        {
            _mediator = mediator;
            _mediaStorage = mediaStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id, CancellationToken cancellationToken)
        {
            if (id <= 0) return NotFound();
            var experience = await _mediator.Send(new GetPublishedExperienceDetailsQuery(id), cancellationToken);
            if (experience is null) return NotFound();

            ViewData["Title"] = experience.Title;
            ViewData["TouristPage"] = "details";
            return View(experience);
        }

        [HttpGet]
        public async Task<IActionResult> Image(int experienceId, int mediaId, CancellationToken cancellationToken)
        {
            if (experienceId <= 0 || mediaId <= 0) return NotFound();
            var key = await _mediator.Send(new GetPublishedExperienceDetailsPhotoKeyQuery(experienceId, mediaId), cancellationToken);
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
