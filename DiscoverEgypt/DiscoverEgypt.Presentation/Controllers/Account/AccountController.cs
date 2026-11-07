using DiscoverEgypt.Application.Features.Accounts.Requests;
using DiscoverEgypt.Application.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace DiscoverEgypt.Presentation.Controllers.Account
{
    public sealed class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new LoginAccountCommand(model), cancellationToken);
                if (!result.Succeeded)
                {
                    AddErrors(result.Errors);
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return result.IsHost ? RedirectToAction("Index", "Experiences") : RedirectToAction(nameof(Welcome));
            }
            catch (FluentValidation.ValidationException exception)
            {
                AddErrors(exception.Errors.Select(error => error.ErrorMessage));
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new RegisterAccountCommand(model), cancellationToken);
                if (!result.Succeeded)
                {
                    AddErrors(result.Errors);
                    return View(model);
                }

                return RedirectToAction(nameof(Welcome));
            }
            catch (FluentValidation.ValidationException exception)
            {
                AddErrors(exception.Errors.Select(error => error.ErrorMessage));
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpGet]
        public IActionResult Welcome() => User.Identity?.IsAuthenticated == true ? View() : Challenge();

        private void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors) ModelState.AddModelError(string.Empty, error);
        }
    }
}

