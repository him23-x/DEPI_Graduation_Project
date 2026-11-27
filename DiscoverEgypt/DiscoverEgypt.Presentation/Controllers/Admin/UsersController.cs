using System.Security.Claims;
using DiscoverEgypt.Application.DTOs.AdminUsers;
using DiscoverEgypt.Application.Enums.AdminUsers;
using DiscoverEgypt.Application.Features.AdminUsers.Requests.Commands;
using DiscoverEgypt.Application.Features.AdminUsers.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Users")]
    public sealed class UsersController : Controller
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var users = await _mediator.Send(new GetAdminUsersQuery(), cancellationToken);
            ViewData["Title"] = "Account management";
            return View("~/Views/Admin/Users/Index.cshtml", users);
        }

        [HttpPost("{id}/status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(
            string id,
            AdminUserStateDto form,
            CancellationToken cancellationToken)
        {
            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(adminUserId)) return Challenge();
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            try
            {
                var result = await _mediator.Send(
                    new SetAdminUserActiveStateCommand(adminUserId, id, form), cancellationToken);
                TempData[result.Outcome == AdminUserStateChangeOutcome.Success ? "AdminUserSuccess" : "AdminUserError"] =
                    result.Outcome == AdminUserStateChangeOutcome.Success
                        ? result.IsActive ? "Account activated. The user can sign in again." : "Account deactivated. New sign-ins are blocked; existing sessions are unchanged."
                        : "This account could not be found.";
            }
            catch (ValidationException exception)
            {
                TempData["AdminUserError"] = exception.Errors.First().ErrorMessage;
            }

            return Redirect("/Admin/Users");
        }
    }
}
