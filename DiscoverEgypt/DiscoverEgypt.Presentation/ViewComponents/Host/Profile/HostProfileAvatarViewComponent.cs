using System.Security.Claims;
using DiscoverEgypt.Application.Features.HostProfile.Requests.Queries;
using DiscoverEgypt.Presentation.ViewModels.Host;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.ViewComponents.Host.Profile
{
    public sealed class HostProfileAvatarViewComponent : ViewComponent
    {
        private readonly IMediator _mediator;

        public HostProfileAvatarViewComponent(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool compact = false)
        {
            var user = HttpContext.User;
            var displayName = user.Identity?.Name ?? "Host";
            var initial = displayName.Length == 0 ? "H" : displayName[..1].ToUpperInvariant();
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var hasProfileImage = !string.IsNullOrWhiteSpace(userId)
                && (await _mediator.Send(new GetHostProfileQuery(userId)))?.HasProfileImage == true;

            return View(new HostProfileAvatarViewModel(displayName, initial, hasProfileImage, compact));
        }
    }
}
