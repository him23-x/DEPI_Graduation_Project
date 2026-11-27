using System.Security.Claims;
using DiscoverEgypt.Application.Features.HostProfile.Requests.Queries;
using DiscoverEgypt.Presentation.ViewModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.ViewComponents.Account
{
    public sealed class AccountMenuViewComponent : ViewComponent
    {
        private readonly IMediator _mediator;

        public AccountMenuViewComponent(IMediator mediator) => _mediator = mediator;

        public async Task<IViewComponentResult> InvokeAsync(string placement = "public")
        {
            var user = HttpContext.User;
            var displayName = user.FindFirstValue(ClaimTypes.GivenName)
                ?? user.Identity?.Name
                ?? "Account";
            displayName = displayName.Trim();
            if (displayName.Length == 0) displayName = "Account";

            var role = user.IsInRole("Admin") ? "Admin"
                : user.IsInRole("Host") ? "Host"
                : user.IsInRole("Guide") ? "Guide"
                : "Tourist";
            var hasProfileImage = false;
            if (role == "Host")
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                hasProfileImage = !string.IsNullOrWhiteSpace(userId)
                    && (await _mediator.Send(new GetHostProfileQuery(userId)))?.HasProfileImage == true;
            }

            return View(new AccountMenuViewModel(
                displayName,
                displayName[..1].ToUpperInvariant(),
                role,
                hasProfileImage,
                placement));
        }
    }
}
