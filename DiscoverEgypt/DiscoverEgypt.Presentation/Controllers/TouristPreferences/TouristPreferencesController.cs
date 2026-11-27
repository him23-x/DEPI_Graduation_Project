using System.Security.Claims;
using DiscoverEgypt.Application.DTOs.TouristPreferences;
using DiscoverEgypt.Application.Features.TouristPreferences.Requests.Commands;
using DiscoverEgypt.Application.Features.TouristPreferences.Requests.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.TouristPreferences
{
    [Authorize(Roles = "Tourist")]
    public sealed class TouristPreferencesController : Controller
    {
        private readonly IMediator _mediator;

        public TouristPreferencesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            ViewData["Title"] = "Your preferences";
            ViewData["TouristPage"] = "preferences";
            return View(await _mediator.Send(new GetTouristPreferenceQuery(userId), cancellationToken));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(TouristPreferenceFormDto model, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();

            ViewData["Title"] = "Your preferences";
            ViewData["TouristPage"] = "preferences";

            if (!ModelState.IsValid)
            {
                await RestoreOptionsAsync(model, userId, cancellationToken);
                return View(model);
            }

            try
            {
                await _mediator.Send(new SaveTouristPreferenceCommand(userId, model), cancellationToken);
                TempData["TouristPreferencesSaved"] = "Your preferences have been saved.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException exception)
            {
                foreach (var error in exception.Errors)
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
            }

            await RestoreOptionsAsync(model, userId, cancellationToken);
            return View(model);
        }

        private async Task RestoreOptionsAsync(TouristPreferenceFormDto model, string userId, CancellationToken cancellationToken)
        {
            var options = await _mediator.Send(new GetTouristPreferenceQuery(userId), cancellationToken);
            model.BudgetOptions = options.BudgetOptions;
            model.DurationOptions = options.DurationOptions;
            model.GuidePreferenceOptions = options.GuidePreferenceOptions;
            model.Interests = options.Interests;
            model.ExperienceTypes = options.ExperienceTypes;
            model.CulturalAreas = options.CulturalAreas;
            model.Languages = options.Languages;
        }
    }
}
