using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Enums.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Services;
using DiscoverEgypt.Application.Features.HostExperiences.Validators.DTOs;
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Tests;

public sealed class ExperienceReadinessServiceTests
{
    private readonly IExperienceReadinessService _service =
        new ExperienceReadinessService(new HostExperienceSubmissionReviewValidator());

    [Fact]
    public void GetIssues_ReturnsNoIssues_ForCompleteFoodExperience()
    {
        var review = CreateReview(
            EvidenceExperienceType.Food,
            new HostExperienceEvidenceSummaryDto(EvidenceKind.MenuAndAllergenInfo, "Vegetarian menu", 0),
            new HostExperienceEvidenceSummaryDto(EvidenceKind.FoodAreaPhoto, null, 3));

        var issues = _service.GetIssues(review);

        Assert.Empty(issues);
    }

    [Fact]
    public void GetIssues_ReportsMissingArtsAndCraftsEvidence()
    {
        var review = CreateReview(EvidenceExperienceType.Arts);

        var issues = _service.GetIssues(review);

        Assert.Contains("Upload 3–10 photos of completed work.", issues);
        Assert.Contains("Upload 1–5 workshop, tool and material photos.", issues);
    }

    [Fact]
    public void GetIssues_RequiresVenueChoice_ForLocalTraditions()
    {
        var review = CreateReview(
            EvidenceExperienceType.LocalTraditions,
            new HostExperienceEvidenceSummaryDto(EvidenceKind.RouteDetails, "A walk through the old market", 0));

        var issues = _service.GetIssues(review);

        Assert.Contains(
            "Confirm whether this experience is a public route, private/home venue, or controlled third-party venue.",
            issues);
    }

    private static HostExperienceSubmissionReviewDto CreateReview(
        EvidenceExperienceType type,
        params HostExperienceEvidenceSummaryDto[] evidence)
    {
        var typeName = type switch
        {
            EvidenceExperienceType.Food => "Food & Cooking",
            EvidenceExperienceType.Arts => "Arts & Crafts",
            EvidenceExperienceType.LocalTraditions => "Local Traditions & Daily Life",
            _ => "Unknown"
        };

        return new HostExperienceSubmissionReviewDto(
            1,
            "A meaningful local experience",
            typeName,
            type,
            "Cairo",
            "A complete description of the experience.",
            "A complete cultural story behind the experience.",
            500m,
            180,
            6,
            "Historic Cairo",
            "A private meeting point for the host and confirmed guests.",
            ExperienceStatus.Draft,
            null,
            1,
            0,
            true,
            evidence,
            true,
            true,
            []);
    }
}
