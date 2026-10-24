using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories;

public sealed class HostExperienceEvidenceRepository(DiscoverEgyptDbContext dbContext) : IHostExperienceEvidenceRepository
{
    public async Task<HostExperienceEvidenceEditor?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default)
    {
        var experience = await dbContext.Experiences.AsNoTracking()
            .Where(item => item.Id == experienceId && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected))
            .Select(item => new
            {
                item.Id,
                item.Title,
                TypeName = item.ExperienceType.Name,
                Evidence = item.Evidence.Where(evidence => evidence.DeletedAt == null)
                    .Select(evidence => new { evidence.Id, evidence.EvidenceKind, evidence.TextContent, evidence.UploadedAt })
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (experience is null) return null;
        var textKind = GetTextKind(Classify(experience.TypeName));
        return new HostExperienceEvidenceEditor(
            experience.Id,
            experience.Title,
            experience.TypeName,
            Classify(experience.TypeName),
            textKind is null ? null : experience.Evidence.FirstOrDefault(item => item.EvidenceKind == textKind)?.TextContent,
            experience.Evidence.Where(item => item.TextContent is null)
                .OrderBy(item => item.EvidenceKind).ThenBy(item => item.UploadedAt)
                .Select(item => new HostExperienceEvidenceFile(item.Id, item.EvidenceKind, item.UploadedAt)).ToArray());
    }

    public async Task<ExperienceEvidenceMutationResult> SaveTextAsync(string userId, int experienceId, EvidenceKind kind, string text, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var experience = await dbContext.Experiences.Include(item => item.Evidence)
            .SingleOrDefaultAsync(item => item.Id == experienceId && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected), cancellationToken);
        if (experience is null) return ExperienceEvidenceMutationResult.ExperienceUnavailable;
        if (GetTextKind(Classify(experience.ExperienceType.Name)) != kind) return ExperienceEvidenceMutationResult.InvalidEvidenceKind;
        var normalized = text.Trim();
        if (normalized.Length == 0) return ExperienceEvidenceMutationResult.TextRequired;
        if (normalized.Length > 5000) return ExperienceEvidenceMutationResult.TextTooLong;

        var existing = experience.Evidence.FirstOrDefault(item => item.EvidenceKind == kind && item.DeletedAt == null && item.TextContent != null);
        if (existing is null)
        {
            experience.Evidence.Add(new ExperienceEvidence { EvidenceKind = kind, TextContent = normalized, UploadedAt = DateTime.UtcNow });
        }
        else
        {
            existing.TextContent = normalized;
            existing.UploadedAt = DateTime.UtcNow;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ExperienceEvidenceMutationResult.Success;
    }

    public async Task<ExperienceEvidenceMutationResult> AddFileAsync(string userId, int experienceId, EvidenceKind kind, string storageKey, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var experience = await dbContext.Experiences.Include(item => item.Evidence)
            .SingleOrDefaultAsync(item => item.Id == experienceId && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected), cancellationToken);
        if (experience is null) return ExperienceEvidenceMutationResult.ExperienceUnavailable;

        var experienceType = Classify(experience.ExperienceType.Name);
        var fileKindAllowed = IsAllowedFileKind(experienceType, kind);
        if (!fileKindAllowed) return ExperienceEvidenceMutationResult.InvalidEvidenceKind;

        var activeCount = experience.Evidence.Count(item => item.EvidenceKind == kind && item.DeletedAt == null && item.StorageKey != null);
        var limit = kind switch
        {
            EvidenceKind.FoodAreaPhoto => 5,
            EvidenceKind.PortfolioPhoto => 10,
            EvidenceKind.WorkshopPhoto => 5,
            EvidenceKind.VenueProof or EvidenceKind.VenuePermission => 1,
            _ => 0
        };
        if (limit == 0 || activeCount >= limit) return ExperienceEvidenceMutationResult.FileLimitReached;

        experience.Evidence.Add(new ExperienceEvidence { EvidenceKind = kind, StorageKey = storageKey, UploadedAt = DateTime.UtcNow });
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ExperienceEvidenceMutationResult.Success;
    }

    public async Task<(ExperienceEvidenceMutationResult Result, string? StorageKey)> DeleteFileAsync(string userId, int experienceId, int evidenceId, CancellationToken cancellationToken = default)
    {
        var evidence = await dbContext.ExperienceEvidence
            .SingleOrDefaultAsync(item => item.Id == evidenceId && item.ExperienceId == experienceId
                && item.Experience.HostProfile.UserId == userId
                && (item.Experience.Status == ExperienceStatus.Draft || item.Experience.Status == ExperienceStatus.Rejected)
                && item.DeletedAt == null && item.StorageKey != null, cancellationToken);
        if (evidence is null) return (ExperienceEvidenceMutationResult.EvidenceUnavailable, null);
        evidence.DeletedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return (ExperienceEvidenceMutationResult.Success, evidence.StorageKey);
    }

    public async Task<StoredExperienceEvidence?> GetOwnedFileAsync(string userId, int evidenceId, CancellationToken cancellationToken = default)
    {
        var key = await dbContext.ExperienceEvidence.AsNoTracking()
            .Where(item => item.Id == evidenceId && item.Experience.HostProfile.UserId == userId
                && item.DeletedAt == null && item.StorageKey != null)
            .Select(item => item.StorageKey)
            .SingleOrDefaultAsync(cancellationToken);
        return key is null ? null : new StoredExperienceEvidence(key);
    }

    private static EvidenceExperienceType Classify(string name)
    {
        var normalized = name.Trim();
        if (normalized.Equals("Food & Cooking", StringComparison.OrdinalIgnoreCase) || normalized.Equals("Food and Cooking", StringComparison.OrdinalIgnoreCase)) return EvidenceExperienceType.Food;
        if (normalized.Equals("Arts & Crafts", StringComparison.OrdinalIgnoreCase) || normalized.Equals("Arts and Crafts", StringComparison.OrdinalIgnoreCase)) return EvidenceExperienceType.Arts;
        if (normalized.Equals("Local Traditions & Daily Life", StringComparison.OrdinalIgnoreCase) || normalized.Equals("Local Traditions and Daily Life", StringComparison.OrdinalIgnoreCase)) return EvidenceExperienceType.LocalTraditions;
        return EvidenceExperienceType.Unsupported;
    }

    private static EvidenceKind? GetTextKind(EvidenceExperienceType type) => type switch
    {
        EvidenceExperienceType.Food => EvidenceKind.MenuAndAllergenInfo,
        EvidenceExperienceType.LocalTraditions => EvidenceKind.RouteDetails,
        _ => null
    };

    private static bool IsAllowedFileKind(EvidenceExperienceType type, EvidenceKind kind) => type switch
    {
        EvidenceExperienceType.Food => kind == EvidenceKind.FoodAreaPhoto,
        EvidenceExperienceType.Arts => kind is EvidenceKind.PortfolioPhoto or EvidenceKind.WorkshopPhoto,
        EvidenceExperienceType.LocalTraditions => kind is EvidenceKind.VenueProof or EvidenceKind.VenuePermission,
        _ => false
    };
}
