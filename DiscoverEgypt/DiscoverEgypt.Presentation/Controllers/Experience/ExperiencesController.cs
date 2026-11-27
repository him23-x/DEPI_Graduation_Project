using System.Security.Claims;
using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.Logging;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Presentation.Controllers.Experience
{
    [Authorize(Roles = "Host")]
    public sealed class ExperiencesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IExperienceReadinessService _readinessService;
        private readonly ILogger<ExperiencesController> _logger;

        public ExperiencesController(
            IMediator mediator,
            IExperienceReadinessService readinessService,
            ILogger<ExperiencesController> logger)
        {
            _mediator = mediator;
            _readinessService = readinessService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(ExperienceStatus? status, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var items = await _mediator.Send(new GetHostExperienceListQuery(userId, status), cancellationToken);
            var model = new ExperiencesIndexDto
            {
                SelectedStatus = status,
                Experiences = items
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var options = await _mediator.Send(new GetHostExperienceCreateOptionsQuery(), cancellationToken);
            var model = new CreateExperienceDto
            {
                Options = options
            };
            ViewData["Section"] = "Create experience";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var editor = await _mediator.Send(new GetEditableHostExperienceDraftQuery(userId, id), cancellationToken);
            if (editor is null) return NotFound();
            var model = new CreateExperienceDto
            {
                ExperienceId = editor.ExperienceId,
                Status = editor.Status,
                RejectionReason = editor.RejectionReason,
                ExperienceTypeId = editor.Draft.ExperienceTypeId,
                CulturalAreaId = editor.Draft.CulturalAreaId,
                Title = editor.Draft.Title,
                Description = editor.Draft.Description,
                CulturalStory = editor.Draft.CulturalStory,
                PricePerPerson = editor.Draft.PricePerPerson,
                DurationMinutes = editor.Draft.DurationMinutes,
                MaxGroupSize = editor.Draft.MaxGroupSize,
                MeetingPointArea = editor.Draft.MeetingPointArea,
                ExactMeetingPoint = editor.Draft.ExactMeetingPoint,
                InterestIds = editor.Draft.InterestIds.ToArray(),
                LanguageIds = editor.Draft.LanguageIds.ToArray()
            };
            model.Options = await _mediator.Send(new GetHostExperienceCreateOptionsQuery(), cancellationToken);
            ViewData["Section"] = "Edit experience";
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateExperienceDto model, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            model.Options = await _mediator.Send(new GetHostExperienceCreateOptionsQuery(), cancellationToken);
            ViewData["Section"] = "Create experience";

            int? experienceId;
            try
            {
                experienceId = await _mediator.Send(new CreateHostExperienceDraftCommand(
                    userId, ToDraft(model)), cancellationToken);
            }
            catch (ValidationException exception)
            {
                AddDraftValidationErrors(exception);
                return View(model);
            }

            if (!experienceId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Your host profile or one of the selected options is no longer available. Refresh the page and try again.");
                return View(model);
            }

            TempData["ExperienceNotice"] = "Your experience draft has been saved. Add at least one photo to continue.";
            return RedirectToAction(nameof(Media), new { id = experienceId.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateExperienceDto model, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            model.ExperienceId = id;
            var editor = await _mediator.Send(new GetEditableHostExperienceDraftQuery(userId, id), cancellationToken);
            if (editor is null) return NotFound();
            model.Status = editor.Status;
            model.RejectionReason = editor.RejectionReason;
            model.Options = await _mediator.Send(new GetHostExperienceCreateOptionsQuery(), cancellationToken);
            ViewData["Section"] = "Edit experience";
            bool updated;
            try
            {
                updated = await _mediator.Send(new UpdateHostExperienceDraftCommand(
                    userId, id, ToDraft(model)), cancellationToken);
            }
            catch (ValidationException exception)
            {
                AddDraftValidationErrors(exception);
                return View("Create", model);
            }
            if (!updated)
            {
                ModelState.AddModelError(string.Empty, "The experience or one of its selected options is no longer available. Refresh and try again.");
                return View("Create", model);
            }

            TempData["ExperienceNotice"] = "Your experience details have been saved. Continue with its media, evidence, and review.";
            return RedirectToAction(nameof(Media), new { id });
        }

        private void AddDraftValidationErrors(ValidationException exception)
        {
            foreach (var failure in exception.Errors)
            {
                var propertyName = failure.PropertyName.StartsWith("Draft.", StringComparison.Ordinal)
                    ? failure.PropertyName["Draft.".Length..]
                    : failure.PropertyName;
                ModelState.AddModelError(propertyName, failure.ErrorMessage);
            }
        }

        private static HostExperienceDraftDto ToDraft(CreateExperienceDto model) => new()
        {
            ExperienceTypeId = model.ExperienceTypeId,
            CulturalAreaId = model.CulturalAreaId,
            Title = model.Title,
            Description = model.Description,
            CulturalStory = model.CulturalStory,
            PricePerPerson = model.PricePerPerson,
            DurationMinutes = model.DurationMinutes,
            MaxGroupSize = model.MaxGroupSize,
            MeetingPointArea = model.MeetingPointArea,
            ExactMeetingPoint = model.ExactMeetingPoint,
            InterestIds = model.InterestIds,
            LanguageIds = model.LanguageIds
        };

        [HttpGet]
        public async Task<IActionResult> Media(int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var editor = await _mediator.Send(new GetHostExperienceMediaEditorQuery(userId, id), cancellationToken);
            if (editor is null)
            {
                return NotFound();
            }

            ViewData["Section"] = "Experience media";
            return View(editor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(300_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 300_000_000)]
        public async Task<IActionResult> UploadMedia(int id, List<IFormFile> files, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }
            if (files.Count == 0)
            {
                TempData["MediaError"] = "Choose at least one photo or MP4 video.";
                return RedirectToAction(nameof(Media), new { id });
            }

            var savedCount = 0;
            var failures = new List<string>();
            foreach (var file in files)
            {
                await using var stream = file.OpenReadStream();
                try
                {
                    var result = await _mediator.Send(new AddHostExperienceMediaCommand(
                        userId, id, file.FileName, file.ContentType, file.Length, stream), cancellationToken);
                    if (result == ExperienceMediaMutationOutcome.Success)
                    {
                        savedCount++;
                    }
                    else
                    {
                        failures.Add(MediaResultMessage(result));
                    }
                }
                catch (InvalidOperationException exception)
                {
                    _logger.LogError(exception, "Experience media upload failed because private storage is unavailable or misconfigured.");
                    failures.Add("Media storage is unavailable or not private. Configure a private Azure container and try again.");
                    break;
                }
            }

            TempData[savedCount > 0 ? "MediaNotice" : "MediaError"] = savedCount > 0
                ? $"Uploaded {savedCount} file{(savedCount == 1 ? "" : "s")}." + (failures.Count > 0 ? $" {string.Join(" ", failures.Distinct())}" : string.Empty)
                : string.Join(" ", failures.Distinct());
            return RedirectToAction(nameof(Media), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCover(int id, int mediaId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var result = await _mediator.Send(new SetHostExperienceMediaCoverCommand(userId, id, mediaId), cancellationToken);
            TempData[result == ExperienceMediaMutationOutcome.Success ? "MediaNotice" : "MediaError"] =
                result == ExperienceMediaMutationOutcome.Success ? "Cover photo updated." : MediaResultMessage(result);
            return RedirectToAction(nameof(Media), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveMedia(int id, int mediaId, int direction, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var result = await _mediator.Send(new MoveHostExperienceMediaCommand(userId, id, mediaId, direction), cancellationToken);
            if (result != ExperienceMediaMutationOutcome.Success)
            {
                TempData["MediaError"] = MediaResultMessage(result);
            }
            return RedirectToAction(nameof(Media), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedia(int id, int mediaId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var result = await _mediator.Send(new DeleteHostExperienceMediaCommand(userId, id, mediaId), cancellationToken);
            TempData[result == ExperienceMediaMutationOutcome.Success ? "MediaNotice" : "MediaError"] =
                result == ExperienceMediaMutationOutcome.Success ? "Media removed from this draft." : MediaResultMessage(result);
            return RedirectToAction(nameof(Media), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> MediaFile(int mediaId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var download = await _mediator.Send(new GetHostExperienceMediaFileQuery(userId, mediaId), cancellationToken);
            if (download is null) return NotFound();
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return File(download.Content, download.ContentType, enableRangeProcessing: true);
        }

        [HttpGet]
        public async Task<IActionResult> Evidence(int id, ExperienceVenueUse? venueUse, bool? evidenceInHostName, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var editor = await _mediator.Send(new GetHostExperienceEvidenceEditorQuery(userId, id), cancellationToken);
            if (editor is null) return NotFound();
            ViewData["Section"] = "Experience evidence";
            var inferredVenueUse = editor.Files.Any(item => item.Kind == EvidenceKind.VenueProof) ? "PrivateHome"
                : editor.Files.Any(item => item.Kind == EvidenceKind.VenuePermission) ? "ThirdPartyControlled" : "PublicRoute";
            var selectedVenueUse = venueUse ?? Enum.Parse<ExperienceVenueUse>(inferredVenueUse);
            var applicationModel = new ExperienceEvidenceDto
            {
                Editor = editor,
                TextContent = editor.TextContent,
                VenueUse = selectedVenueUse,
                EvidenceInHostName = evidenceInHostName ?? !editor.Files.Any(item => item.Kind == EvidenceKind.VenuePermission)
            };
            return View(applicationModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEvidenceText(int id, SaveExperienceEvidenceTextDto model, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            model.ExperienceId = id;
            try
            {
                var result = await _mediator.Send(new SaveHostExperienceEvidenceTextCommand(userId, id, model.Kind, model.TextContent), cancellationToken);
                TempData[result == ExperienceEvidenceMutationOutcome.Success ? "EvidenceNotice" : "EvidenceError"] = EvidenceResultMessage(result);
            }
            catch (FluentValidation.ValidationException exception)
            {
                TempData["EvidenceError"] = exception.Errors.First().ErrorMessage;
            }
            return RedirectToAction(nameof(Evidence), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(20_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 20_000_000)]
        public async Task<IActionResult> UploadEvidence(int id, EvidenceKind kind, IFormFile? file, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            if (file is null || file.Length == 0)
            {
                TempData["EvidenceError"] = "Choose a PDF, JPEG, or PNG file up to 10 MB.";
                return RedirectToAction(nameof(Evidence), new { id });
            }
            await using var stream = file.OpenReadStream();
            try
            {
                var result = await _mediator.Send(new AddHostExperienceEvidenceFileCommand(
                    userId, id, kind, file.FileName, file.ContentType, file.Length, stream), cancellationToken);
                TempData[result == ExperienceEvidenceMutationOutcome.Success ? "EvidenceNotice" : "EvidenceError"] = EvidenceResultMessage(result);
            }
            catch (InvalidOperationException)
            {
                TempData["EvidenceError"] = "Evidence storage is unavailable or not private. Configure its private Azure container and try again.";
            }
            return RedirectToAction(nameof(Evidence), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvidence(int id, int evidenceId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var result = await _mediator.Send(new DeleteHostExperienceEvidenceFileCommand(userId, id, evidenceId), cancellationToken);
            TempData[result == ExperienceEvidenceMutationOutcome.Success ? "EvidenceNotice" : "EvidenceError"] =
                result == ExperienceEvidenceMutationOutcome.Success ? "Evidence file removed from this draft." : EvidenceResultMessage(result);
            return RedirectToAction(nameof(Evidence), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> EvidenceFile(int evidenceId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var download = await _mediator.Send(new GetHostExperienceEvidenceFileQuery(userId, evidenceId), cancellationToken);
            if (download is null) return NotFound();
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            return File(download.Content, "application/octet-stream", $"experience-evidence-{evidenceId}");
        }

        [HttpGet]
        public async Task<IActionResult> Review(int id, ExperienceVenueUse? venueUse, bool? evidenceInHostName, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            var review = await _mediator.Send(new GetHostExperienceSubmissionReviewQuery(userId, id), cancellationToken);
            if (review is null) return NotFound();
            var inferredVenueUse = review.Evidence.Any(item => item.Kind == EvidenceKind.VenueProof && item.FileCount > 0)
                ? ExperienceVenueUse.PrivateHome
                : review.Evidence.Any(item => item.Kind == EvidenceKind.VenuePermission && item.FileCount > 0)
                    ? ExperienceVenueUse.ThirdPartyControlled : ExperienceVenueUse.PublicRoute;
            var selectedVenueUse = venueUse ?? inferredVenueUse;
            var addressDocumentInHostName = evidenceInHostName ?? !review.Evidence.Any(item => item.Kind == EvidenceKind.VenuePermission && item.FileCount > 0);
            var reviewIssues = _readinessService.GetIssues(review, venueUse, addressDocumentInHostName).ToList();
            review = review with { Issues = reviewIssues };
            var declaration = HostExperienceSubmissionPolicy.GetDeclaration(review.ExperienceType);
            ViewData["Section"] = "Review experience";
            var applicationModel = new ExperienceReviewDto
            {
                Review = review,
                DeclarationText = declaration?.Text ?? string.Empty,
                VenueUse = selectedVenueUse,
                EvidenceInHostName = addressDocumentInHostName
            };
            return View(applicationModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int id, SubmitExperienceDto model, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Challenge();
            model.ExperienceId = id;
            ExperienceSubmissionOutcome result;
            try
            {
                result = await _mediator.Send(new SubmitHostExperienceCommand(userId, id, model.Accepted, model.VenueUse, model.EvidenceInHostName), cancellationToken);
            }
            catch (FluentValidation.ValidationException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Errors.First().ErrorMessage);
                result = model.Accepted ? ExperienceSubmissionOutcome.Incomplete : ExperienceSubmissionOutcome.DeclarationNotAccepted;
            }
            if (result == ExperienceSubmissionOutcome.Success)
            {
                TempData["ExperienceNotice"] = "Your experience has been submitted for review.";
                return RedirectToAction(nameof(Index));
            }
            if (result == ExperienceSubmissionOutcome.ExperienceUnavailable) return NotFound();
            var review = await _mediator.Send(new GetHostExperienceSubmissionReviewQuery(userId, id), cancellationToken);
            if (review is null) return NotFound();
            var venueUse = model.VenueUse;
            var selectedVenueUse = venueUse ?? (review.Evidence.Any(item => item.Kind == EvidenceKind.VenueProof && item.FileCount > 0)
                ? ExperienceVenueUse.PrivateHome
                : review.Evidence.Any(item => item.Kind == EvidenceKind.VenuePermission && item.FileCount > 0)
                    ? ExperienceVenueUse.ThirdPartyControlled : ExperienceVenueUse.PublicRoute);
            var issues = _readinessService.GetIssues(review, venueUse, model.EvidenceInHostName).ToList();
            review = review with { Issues = issues };
            var declaration = HostExperienceSubmissionPolicy.GetDeclaration(review.ExperienceType);
            ViewData["Section"] = "Review experience";
            var applicationReview = new ExperienceReviewDto
            {
                Review = review,
                DeclarationText = declaration?.Text ?? string.Empty,
                VenueUse = selectedVenueUse,
                EvidenceInHostName = model.EvidenceInHostName,
                Error = result switch
                {
                    ExperienceSubmissionOutcome.DeclarationNotAccepted => "Accept the declaration before submitting.",
                    ExperienceSubmissionOutcome.Incomplete => "Complete the required listing, media, venue, and evidence items below before submitting.",
                    ExperienceSubmissionOutcome.UnsupportedType => "This experience category does not have a configured submission declaration.",
                    _ => "This experience cannot be submitted in its current state. Pending approval and unpublished experiences are read-only."
                }
            };
            return View(nameof(Review), applicationReview);
        }

        private static string MediaResultMessage(ExperienceMediaMutationOutcome result) => result switch
        {
            ExperienceMediaMutationOutcome.ExperienceUnavailable => "Only your own draft or rejected experiences can be changed. Pending approval and unpublished experiences are read-only.",
            ExperienceMediaMutationOutcome.ImageLimitReached => "An experience can have up to 8 images.",
            ExperienceMediaMutationOutcome.VideoLimitReached => "An experience can have up to 2 videos.",
            ExperienceMediaMutationOutcome.InvalidFile => "Use JPEG, PNG, or WebP images up to 10 MB, or MP4 videos up to 100 MB.",
            ExperienceMediaMutationOutcome.MediaUnavailable => "That media item is no longer available.",
            ExperienceMediaMutationOutcome.CoverMustBeImage => "Choose an image for the cover photo.",
            ExperienceMediaMutationOutcome.InvalidOrder => "That item cannot move any farther in that direction.",
            _ => "The media change could not be completed. Refresh and try again."
        };

        private static string EvidenceResultMessage(ExperienceEvidenceMutationOutcome result) => result switch
        {
            ExperienceEvidenceMutationOutcome.Success => "Evidence saved to your private draft.",
            ExperienceEvidenceMutationOutcome.ExperienceUnavailable => "Only your own draft or rejected experiences can be changed. Pending approval and unpublished experiences are read-only.",
            ExperienceEvidenceMutationOutcome.EvidenceUnavailable => "That evidence file is no longer available.",
            ExperienceEvidenceMutationOutcome.InvalidEvidenceKind => "This evidence type does not apply to the selected experience category.",
            ExperienceEvidenceMutationOutcome.InvalidFile => "Use a valid PDF, JPEG, or PNG file up to 10 MB.",
            ExperienceEvidenceMutationOutcome.FileLimitReached => "The maximum number of files for this evidence category has been reached.",
            ExperienceEvidenceMutationOutcome.TextRequired => "Enter the requested details before saving.",
            ExperienceEvidenceMutationOutcome.TextTooLong => "Keep the details to 5,000 characters or fewer.",
            _ => "The evidence change could not be completed. Refresh and try again."
        };

    }
}

