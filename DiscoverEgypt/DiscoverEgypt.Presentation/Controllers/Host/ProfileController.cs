using System.Security.Claims;
using DiscoverEgypt.Application.Features.HostProfile.Requests.Commands;
using DiscoverEgypt.Application.Features.HostProfile.Requests.Queries;
using DiscoverEgypt.Application.DTOs.HostProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Host
{
    [Authorize(Roles = "Host")]
    public sealed class ProfileController : Controller
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            var profile = await _mediator.Send(new GetHostProfileQuery(userId), cancellationToken)
                ?? new HostProfileDto();
            ViewData["Section"] = "Profile";
            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Image(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            var image = await _mediator.Send(new GetHostProfileImageQuery(userId), cancellationToken);
            if (image is null) return NotFound();

            Response.Headers.CacheControl = "private, max-age=0, must-revalidate";
            return File(image.Content, image.ContentType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(22_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 22_000_000)]
        public async Task<IActionResult> Index(HostProfileDto model, IFormFile? profileImage, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            try
            {
                await using var imageContent = profileImage?.OpenReadStream();
                await _mediator.Send(new SaveHostProfileCommand(
                    userId,
                    model,
                    imageContent,
                    profileImage?.ContentType,
                    profileImage is null ? null : Path.GetExtension(profileImage.FileName).ToLowerInvariant(),
                    profileImage?.Length ?? 0), cancellationToken);
                TempData["HostProfileSaved"] = "Your host profile has been saved.";
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException exception)
            {
                foreach (var error in exception.Errors)
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
            }

            var currentProfile = await _mediator.Send(new GetHostProfileQuery(userId), cancellationToken);
            model.HasProfileImage = currentProfile?.HasProfileImage ?? false;
            ViewData["Section"] = "Profile";
            return View(model);
        }
    }
}
