using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Validation;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Host;

public sealed class HostExperienceSubmissionRepository(
    DiscoverEgyptDbContext dbContext,
    HostExperienceSubmissionReviewValidator reviewValidator) : IHostExperienceSubmissionRepository
{
    public async Task<HostExperienceSubmissionReview?> GetReviewAsync(string userId, int experienceId, CancellationToken cancellationToken = default)
    {
        var experience = await GetEditableExperience(userId, experienceId, tracked: false).SingleOrDefaultAsync(cancellationToken);
        return experience is null ? null : ToReview(experience);
    }

    public async Task<ExperienceSubmissionResult> SubmitAsync(string userId, int experienceId, DeclarationType declarationType, string declarationText, ExperienceVenueUse? venueUse, bool evidenceInHostName, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var experience = await GetEditableExperience(userId, experienceId, tracked: true).SingleOrDefaultAsync(cancellationToken);
        if (experience is null) return ExperienceSubmissionResult.ExperienceUnavailable;

        var expectedDeclaration = HostExperienceSubmissionPolicy.GetDeclaration(HostExperienceSubmissionPolicy.Classify(experience.ExperienceType.Name));
        if (expectedDeclaration is null) return ExperienceSubmissionResult.UnsupportedType;
        if (HostExperienceSubmissionPolicy.Classify(experience.ExperienceType.Name) == EvidenceExperienceType.LocalTraditions
            && (!venueUse.HasValue || !Enum.IsDefined(venueUse.Value))) return ExperienceSubmissionResult.Incomplete;
        if (declarationType != expectedDeclaration.Type || !string.Equals(declarationText, expectedDeclaration.Text, StringComparison.Ordinal))
            return ExperienceSubmissionResult.DeclarationNotAccepted;
        if (reviewValidator.GetIssues(ToReview(experience), venueUse, evidenceInHostName).Count > 0)
            return ExperienceSubmissionResult.Incomplete;

        var now = DateTime.UtcNow;
        dbContext.ExperienceDeclarations.Add(new ExperienceDeclaration
        {
            ExperienceId = experience.Id,
            HostUserId = userId,
            DeclarationType = expectedDeclaration.Type,
            DeclarationText = expectedDeclaration.Text,
            AcceptedAt = now
        });
        experience.Status = ExperienceStatus.PendingApproval;
        experience.RejectionReason = null;
        experience.UpdatedAt = now;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ExperienceSubmissionResult.Success;
    }

    private IQueryable<Experience> GetEditableExperience(string userId, int experienceId, bool tracked)
    {
        IQueryable<Experience> query = dbContext.Experiences;
        if (!tracked) query = query.AsNoTracking();
        return query
            .Where(item => item.Id == experienceId && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected))
            .Include(item => item.ExperienceType)
            .Include(item => item.CulturalArea)
            .Include(item => item.Media)
            .Include(item => item.Evidence.Where(evidence => evidence.DeletedAt == null))
            .Include(item => item.ExperienceInterests)
            .Include(item => item.ExperienceLanguages);
    }

    private static HostExperienceSubmissionReview ToReview(Experience item)
    {
        var evidence = item.Evidence
            .GroupBy(row => row.EvidenceKind)
            .Select(group => new HostExperienceEvidenceSummary(
                group.Key,
                group.FirstOrDefault(row => row.TextContent != null)?.TextContent,
                group.Count(row => row.StorageKey != null)))
            .OrderBy(row => row.Kind)
            .ToArray();
        return new HostExperienceSubmissionReview(
            item.Id,
            item.Title,
            item.ExperienceType.Name,
            HostExperienceSubmissionPolicy.Classify(item.ExperienceType.Name),
            item.CulturalArea.Name,
            item.Description,
            item.CulturalStory,
            item.PricePerPerson,
            item.DurationMinutes,
            item.MaxGroupSize,
            item.MeetingPointArea,
            item.ExactMeetingPoint,
            item.Status,
            item.RejectionReason,
            item.Media.Count(media => media.MediaType == MediaType.Image),
            item.Media.Count(media => media.MediaType == MediaType.Video),
            item.Media.Any(media => media.MediaType == MediaType.Image && media.IsCover),
            evidence,
            item.ExperienceInterests.Count > 0,
            item.ExperienceLanguages.Count > 0,
            []);
    }
}
