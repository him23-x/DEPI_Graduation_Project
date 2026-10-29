using DiscoverEgypt.Application.DTOs.HostExperiences;
using FluentValidation;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators.DTOs;

public sealed class HostExperienceSubmissionReviewValidator : AbstractValidator<HostExperienceSubmissionReviewDto>
{
    private const string VenueUseKey = "VenueUse";
    private const string EvidenceInHostNameKey = "EvidenceInHostName";

    public HostExperienceSubmissionReviewValidator()
    {
        RuleFor(review => review).Custom((review, context) =>
        {
            ExperienceVenueUse? venueUse = context.RootContextData.TryGetValue(VenueUseKey, out var value)
                && value is ExperienceVenueUse selectedVenueUse
                    ? selectedVenueUse
                    : null;
            var evidenceInHostName = !context.RootContextData.TryGetValue(EvidenceInHostNameKey, out var nameValue)
                || nameValue is not bool isInHostName
                || isInHostName;

            foreach (var issue in CollectIssues(review, venueUse, evidenceInHostName))
                context.AddFailure(issue);
        });
    }

    public IReadOnlyList<string> GetIssues(
        HostExperienceSubmissionReviewDto review,
        ExperienceVenueUse? selectedVenueUse = null,
        bool evidenceInHostName = true)
    {
        var validationContext = new ValidationContext<HostExperienceSubmissionReviewDto>(review);
        if (selectedVenueUse.HasValue)
            validationContext.RootContextData[VenueUseKey] = selectedVenueUse.Value;
        validationContext.RootContextData[EvidenceInHostNameKey] = evidenceInHostName;
        return Validate(validationContext).Errors.Select(error => error.ErrorMessage).ToArray();
    }

    private static IReadOnlyList<string> CollectIssues(
        HostExperienceSubmissionReviewDto review,
        ExperienceVenueUse? selectedVenueUse,
        bool evidenceInHostName)
    {
        var issues = new List<string>();
        if (string.IsNullOrWhiteSpace(review.Title)) issues.Add("Add an experience title.");
        if (string.IsNullOrWhiteSpace(review.Description)) issues.Add("Add an experience description.");
        if (string.IsNullOrWhiteSpace(review.CulturalStory)) issues.Add("Add the cultural story.");
        if (review.PricePerPerson <= 0) issues.Add("Enter a price greater than zero.");
        if (review.DurationMinutes <= 0 || review.MaxGroupSize <= 0) issues.Add("Check the duration and group size.");
        if (string.IsNullOrWhiteSpace(review.MeetingPointArea) || string.IsNullOrWhiteSpace(review.ExactMeetingPoint)) issues.Add("Add the meeting area and exact meeting point.");
        if (!review.HasInterest) issues.Add("Choose at least one interest.");
        if (!review.HasLanguage) issues.Add("Choose at least one language.");
        if (review.ImageCount < 1 || !review.HasCoverImage) issues.Add("Add at least one experience photo and choose a cover photo.");

        int Files(EvidenceKind kind) => review.Evidence.FirstOrDefault(item => item.Kind == kind)?.FileCount ?? 0;
        string? Text(EvidenceKind kind) => review.Evidence.FirstOrDefault(item => item.Kind == kind)?.TextContent;
        switch (review.ExperienceType)
        {
            case EvidenceExperienceType.Food:
                if (string.IsNullOrWhiteSpace(Text(EvidenceKind.MenuAndAllergenInfo))) issues.Add("Add menu, ingredient and allergen information.");
                if (Files(EvidenceKind.FoodAreaPhoto) is < 3 or > 5) issues.Add("Upload 3–5 food preparation, washing and storage area photos.");
                break;
            case EvidenceExperienceType.Arts:
                if (Files(EvidenceKind.PortfolioPhoto) is < 3 or > 10) issues.Add("Upload 3–10 photos of completed work.");
                if (Files(EvidenceKind.WorkshopPhoto) is < 1 or > 5) issues.Add("Upload 1–5 workshop, tool and material photos.");
                break;
            case EvidenceExperienceType.LocalTraditions:
                if (string.IsNullOrWhiteSpace(Text(EvidenceKind.RouteDetails))) issues.Add("Add the route and location details.");
                if (Files(EvidenceKind.VenueProof) > 1 || Files(EvidenceKind.VenuePermission) > 1) issues.Add("Use no more than one file for each venue evidence type.");
                var venueUse = selectedVenueUse ?? (Files(EvidenceKind.VenueProof) > 0
                    ? ExperienceVenueUse.PrivateHome
                    : Files(EvidenceKind.VenuePermission) > 0 ? ExperienceVenueUse.ThirdPartyControlled : ExperienceVenueUse.PublicRoute);
                if (venueUse == ExperienceVenueUse.PrivateHome && Files(EvidenceKind.VenueProof) != 1)
                    issues.Add("Upload one recent utility bill or address proof for the private or home venue.");
                if (venueUse == ExperienceVenueUse.PrivateHome && !evidenceInHostName && Files(EvidenceKind.VenuePermission) != 1)
                    issues.Add("Upload a lease or written owner permission because the address proof is not in your name.");
                if (venueUse == ExperienceVenueUse.ThirdPartyControlled && Files(EvidenceKind.VenuePermission) != 1)
                    issues.Add("Upload the separate permission for the controlled third-party venue.");
                break;
            default:
                issues.Add($"The experience category ‘{review.ExperienceTypeName}’ is not mapped to a submission checklist.");
                break;
        }
        return issues;
    }
}
