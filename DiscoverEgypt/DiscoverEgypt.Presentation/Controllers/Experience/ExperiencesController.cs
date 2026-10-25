using System.Security.Claims;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Validation;
using FluentValidation;
using DiscoverEgypt.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace DiscoverEgypt.Presentation.Controllers.Experience;

[Authorize(Roles = "Host")]
public sealed class ExperiencesController(
    IMediator mediator,
    HostExperienceSubmissionReviewValidator reviewValidator,
    CreateExperienceDtoValidator createExperienceValidator,
    SaveExperienceEvidenceTextDtoValidator saveEvidenceTextValidator,
    SubmitExperienceDtoValidator submitExperienceValidator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(ExperienceStatus? status, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var items = await mediator.Send(new GetHostExperienceListRequest(userId, status), cancellationToken);
        return View(new ExperiencesIndexDto
        {
            SelectedStatus = status,
            Experiences = items
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new CreateExperienceDto
        {
            Options = await mediator.Send(new GetHostExperienceCreateOptionsQuery(), cancellationToken)
        };
        ViewData["Section"] = "Create experience";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        var editor = await mediator.Send(new GetEditableHostExperienceDraftQuery(userId, id), cancellationToken);
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
            LanguageIds = editor.Draft.LanguageIds.ToArray(),
            Options = await mediator.Send(new GetHostExperienceCreateOptionsQuery(), cancellationToken)
        };
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

        model.Options = await mediator.Send(new GetHostExperienceCreateOptionsQuery(), cancellationToken);
        ViewData["Section"] = "Create experience";

        ApplyValidation(await createExperienceValidator.ValidateAsync(model, cancellationToken));
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        int? experienceId;
        try
        {
            experienceId = await mediator.Send(new CreateHostExperienceDraftCommand(userId, new CreateHostExperienceDraftRequest(
            model.ExperienceTypeId,
            model.CulturalAreaId,
            model.Title,
            model.Description,
            model.CulturalStory,
            model.PricePerPerson,
            model.DurationMinutes,
            model.MaxGroupSize,
            model.MeetingPointArea,
            model.ExactMeetingPoint,
            model.InterestIds,
            model.LanguageIds)), cancellationToken);
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
        var editor = await mediator.Send(new GetEditableHostExperienceDraftQuery(userId, id), cancellationToken);
        if (editor is null) return NotFound();
        model.Status = editor.Status;
        model.RejectionReason = editor.RejectionReason;
        model.Options = await mediator.Send(new GetHostExperienceCreateOptionsQuery(), cancellationToken);
        ViewData["Section"] = "Edit experience";
        ApplyValidation(await createExperienceValidator.ValidateAsync(model, cancellationToken));
        if (!ModelState.IsValid) return View("Create", model);

        bool updated;
        try
        {
            updated = await mediator.Send(new UpdateHostExperienceDraftCommand(userId, id, new CreateHostExperienceDraftRequest(
            model.ExperienceTypeId,
            model.CulturalAreaId,
            model.Title,
            model.Description,
            model.CulturalStory,
            model.PricePerPerson,
            model.DurationMinutes,
            model.MaxGroupSize,
            model.MeetingPointArea,
            model.ExactMeetingPoint,
            model.InterestIds,
            model.LanguageIds)), cancellationToken);
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

    [HttpGet]
    public async Task<IActionResult> Media(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var editor = await mediator.Send(new GetHostExperienceMediaEditorQuery(userId, id), cancellationToken);
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
                var result = await mediator.Send(new AddHostExperienceMediaCommand(
                    userId, id, file.FileName, file.ContentType, file.Length, stream), cancellationToken);
                if (result == ExperienceMediaMutationResult.Success)
                {
                    savedCount++;
                }
                else
                {
                    failures.Add(MediaResultMessage(result));
                }
            }
            catch (InvalidOperationException)
            {
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
        var result = await mediator.Send(new SetHostExperienceMediaCoverCommand(userId, id, mediaId), cancellationToken);
        TempData[result == ExperienceMediaMutationResult.Success ? "MediaNotice" : "MediaError"] =
            result == ExperienceMediaMutationResult.Success ? "Cover photo updated." : MediaResultMessage(result);
        return RedirectToAction(nameof(Media), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MoveMedia(int id, int mediaId, int direction, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        var result = await mediator.Send(new MoveHostExperienceMediaCommand(userId, id, mediaId, direction), cancellationToken);
        if (result != ExperienceMediaMutationResult.Success)
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
        var result = await mediator.Send(new DeleteHostExperienceMediaCommand(userId, id, mediaId), cancellationToken);
        TempData[result == ExperienceMediaMutationResult.Success ? "MediaNotice" : "MediaError"] =
            result == ExperienceMediaMutationResult.Success ? "Media removed from this draft." : MediaResultMessage(result);
        return RedirectToAction(nameof(Media), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> MediaFile(int mediaId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        var download = await mediator.Send(new GetHostExperienceMediaFileQuery(userId, mediaId), cancellationToken);
        if (download is null) return NotFound();
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(download.Content, download.ContentType, enableRangeProcessing: true);
    }

    [HttpGet]
    public async Task<IActionResult> Evidence(int id, ExperienceVenueUse? venueUse, bool? evidenceInHostName, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        var editor = await mediator.Send(new GetHostExperienceEvidenceEditorQuery(userId, id), cancellationToken);
        if (editor is null) return NotFound();
        ViewData["Section"] = "Experience evidence";
        var inferredVenueUse = editor.Files.Any(item => item.Kind == EvidenceKind.VenueProof) ? "PrivateHome"
            : editor.Files.Any(item => item.Kind == EvidenceKind.VenuePermission) ? "ThirdPartyControlled" : "PublicRoute";
        var selectedVenueUse = venueUse ?? Enum.Parse<ExperienceVenueUse>(inferredVenueUse);
        return View(new ExperienceEvidenceDto
        {
            Editor = editor,
            TextContent = editor.TextContent,
            VenueUse = selectedVenueUse,
            EvidenceInHostName = evidenceInHostName ?? !editor.Files.Any(item => item.Kind == EvidenceKind.VenuePermission)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEvidenceText(int id, SaveExperienceEvidenceTextDto model, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        model.ExperienceId = id;
        var validation = await saveEvidenceTextValidator.ValidateAsync(model, cancellationToken);
        if (!validation.IsValid)
        {
            TempData["EvidenceError"] = validation.Errors[0].ErrorMessage;
            return RedirectToAction(nameof(Evidence), new { id });
        }
        var result = await mediator.Send(new SaveHostExperienceEvidenceTextCommand(userId, id, model.Kind, model.TextContent), cancellationToken);
        TempData[result == ExperienceEvidenceMutationResult.Success ? "EvidenceNotice" : "EvidenceError"] = EvidenceResultMessage(result);
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
            var result = await mediator.Send(new AddHostExperienceEvidenceFileCommand(
                userId, id, kind, file.FileName, file.ContentType, file.Length, stream), cancellationToken);
            TempData[result == ExperienceEvidenceMutationResult.Success ? "EvidenceNotice" : "EvidenceError"] = EvidenceResultMessage(result);
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
        var result = await mediator.Send(new DeleteHostExperienceEvidenceFileCommand(userId, id, evidenceId), cancellationToken);
        TempData[result == ExperienceEvidenceMutationResult.Success ? "EvidenceNotice" : "EvidenceError"] =
            result == ExperienceEvidenceMutationResult.Success ? "Evidence file removed from this draft." : EvidenceResultMessage(result);
        return RedirectToAction(nameof(Evidence), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> EvidenceFile(int evidenceId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        var download = await mediator.Send(new GetHostExperienceEvidenceFileQuery(userId, evidenceId), cancellationToken);
        if (download is null) return NotFound();
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(download.Content, "application/octet-stream", $"experience-evidence-{evidenceId}");
    }

    [HttpGet]
    public async Task<IActionResult> Review(int id, ExperienceVenueUse? venueUse, bool? evidenceInHostName, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        var review = await mediator.Send(new GetHostExperienceSubmissionReviewQuery(userId, id), cancellationToken);
        if (review is null) return NotFound();
        var inferredVenueUse = review.Evidence.Any(item => item.Kind == EvidenceKind.VenueProof && item.FileCount > 0)
            ? ExperienceVenueUse.PrivateHome
            : review.Evidence.Any(item => item.Kind == EvidenceKind.VenuePermission && item.FileCount > 0)
                ? ExperienceVenueUse.ThirdPartyControlled : ExperienceVenueUse.PublicRoute;
        var selectedVenueUse = venueUse ?? inferredVenueUse;
        var addressDocumentInHostName = evidenceInHostName ?? !review.Evidence.Any(item => item.Kind == EvidenceKind.VenuePermission && item.FileCount > 0);
        var reviewIssues = reviewValidator.GetIssues(review, venueUse ?? inferredVenueUse, addressDocumentInHostName).ToList();
        review = review with { Issues = reviewIssues };
        var declaration = HostExperienceSubmissionPolicy.GetDeclaration(review.ExperienceType);
        ViewData["Section"] = "Review experience";
        return View(new ExperienceReviewDto
        {
            Review = review,
            DeclarationText = declaration?.Text ?? string.Empty,
            VenueUse = selectedVenueUse,
            EvidenceInHostName = addressDocumentInHostName
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int id, SubmitExperienceDto model, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        model.ExperienceId = id;
        var validation = await submitExperienceValidator.ValidateAsync(model, cancellationToken);
        ApplyValidation(validation);
        var result = validation.IsValid
            ? await mediator.Send(new SubmitHostExperienceCommand(userId, id, model.Accepted, model.VenueUse, model.EvidenceInHostName), cancellationToken)
            : model.Accepted ? ExperienceSubmissionResult.Incomplete : ExperienceSubmissionResult.DeclarationNotAccepted;
        if (result == ExperienceSubmissionResult.Success)
        {
            TempData["ExperienceNotice"] = "Your experience has been submitted for review.";
            return RedirectToAction(nameof(Index));
        }
        if (result == ExperienceSubmissionResult.ExperienceUnavailable) return NotFound();
        var review = await mediator.Send(new GetHostExperienceSubmissionReviewQuery(userId, id), cancellationToken);
        if (review is null) return NotFound();
        var venueUse = model.VenueUse;
        var selectedVenueUse = venueUse ?? (review.Evidence.Any(item => item.Kind == EvidenceKind.VenueProof && item.FileCount > 0)
            ? ExperienceVenueUse.PrivateHome
            : review.Evidence.Any(item => item.Kind == EvidenceKind.VenuePermission && item.FileCount > 0)
                ? ExperienceVenueUse.ThirdPartyControlled : ExperienceVenueUse.PublicRoute);
        var issues = reviewValidator.GetIssues(review, venueUse, model.EvidenceInHostName).ToList();
        if (review.ExperienceType == EvidenceExperienceType.LocalTraditions && !venueUse.HasValue)
            issues.Add("Choose whether this experience is a public route, private/home venue, or controlled third-party venue.");
        review = review with { Issues = issues };
        var declaration = HostExperienceSubmissionPolicy.GetDeclaration(review.ExperienceType);
        ViewData["Section"] = "Review experience";
        return View(nameof(Review), new ExperienceReviewDto
        {
            Review = review,
            DeclarationText = declaration?.Text ?? string.Empty,
            VenueUse = selectedVenueUse,
            EvidenceInHostName = model.EvidenceInHostName,
            Error = result switch
            {
                ExperienceSubmissionResult.DeclarationNotAccepted => "Accept the declaration before submitting.",
                ExperienceSubmissionResult.Incomplete => "Complete the required listing, media, venue, and evidence items below before submitting.",
                ExperienceSubmissionResult.UnsupportedType => "This experience category does not have a configured submission declaration.",
                _ => "This experience cannot be submitted in its current state. Pending approval and unpublished experiences are read-only."
            }
        });
    }

    private void ApplyValidation(FluentValidation.Results.ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }

    private static string MediaResultMessage(ExperienceMediaMutationResult result) => result switch
    {
        ExperienceMediaMutationResult.ExperienceUnavailable => "Only your own draft or rejected experiences can be changed. Pending approval and unpublished experiences are read-only.",
        ExperienceMediaMutationResult.ImageLimitReached => "An experience can have up to 8 images.",
        ExperienceMediaMutationResult.VideoLimitReached => "An experience can have up to 2 videos.",
        ExperienceMediaMutationResult.InvalidFile => "Use JPEG, PNG, or WebP images up to 10 MB, or MP4 videos up to 100 MB.",
        ExperienceMediaMutationResult.MediaUnavailable => "That media item is no longer available.",
        ExperienceMediaMutationResult.CoverMustBeImage => "Choose an image for the cover photo.",
        ExperienceMediaMutationResult.InvalidOrder => "That item cannot move any farther in that direction.",
        _ => "The media change could not be completed. Refresh and try again."
    };

    private static string EvidenceResultMessage(ExperienceEvidenceMutationResult result) => result switch
    {
        ExperienceEvidenceMutationResult.Success => "Evidence saved to your private draft.",
        ExperienceEvidenceMutationResult.ExperienceUnavailable => "Only your own draft or rejected experiences can be changed. Pending approval and unpublished experiences are read-only.",
        ExperienceEvidenceMutationResult.EvidenceUnavailable => "That evidence file is no longer available.",
        ExperienceEvidenceMutationResult.InvalidEvidenceKind => "This evidence type does not apply to the selected experience category.",
        ExperienceEvidenceMutationResult.InvalidFile => "Use a valid PDF, JPEG, or PNG file up to 10 MB.",
        ExperienceEvidenceMutationResult.FileLimitReached => "The maximum number of files for this evidence category has been reached.",
        ExperienceEvidenceMutationResult.TextRequired => "Enter the requested details before saving.",
        ExperienceEvidenceMutationResult.TextTooLong => "Keep the details to 5,000 characters or fewer.",
        _ => "The evidence change could not be completed. Refresh and try again."
    };

}
