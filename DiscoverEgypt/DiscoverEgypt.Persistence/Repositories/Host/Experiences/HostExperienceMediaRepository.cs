using System.Data;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using DiscoverEgypt.Application.Contracts.Storage;

using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Persistence.Repositories.Host.Experiences;

public sealed class HostExperienceMediaRepository(DiscoverEgyptDbContext dbContext) : IHostExperienceMediaRepository
{
    public async Task<HostExperienceMediaEditorDto?> GetDraftEditorAsync(string userId, int experienceId, CancellationToken cancellationToken = default)
    {
        var experience = await dbContext.Experiences
            .AsNoTracking()
            .Where(item => item.Id == experienceId
                && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected))
            .Select(item => new
            {
                item.Id,
                item.Title,
                Media = item.Media.OrderBy(media => media.DisplayOrder).ThenBy(media => media.Id)
                    .Select(media => new { media.Id, media.MediaType, media.DisplayOrder, media.IsCover, media.StorageKey })
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);

        return experience is null
            ? null
            : new HostExperienceMediaEditorDto(experience.Id, experience.Title, experience.Media.Select(media => new HostExperienceMediaItemDto(
                media.Id,
                media.MediaType,
                GetContentType(media.StorageKey, media.MediaType),
                media.DisplayOrder,
                media.IsCover)).ToArray());
    }

    public async Task<StoredExperienceMedia?> GetOwnedMediaAsync(string userId, int mediaId, CancellationToken cancellationToken = default)
    {
        return await dbContext.ExperienceMedia
            .AsNoTracking()
            .Where(media => media.Id == mediaId && media.Experience.HostProfile.UserId == userId)
            .Select(media => new StoredExperienceMedia(media.StorageKey, media.MediaType))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<ExperienceMediaMutationOutcome> AddAsync(string userId, int experienceId, MediaType mediaType, string storageKey, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var experience = await dbContext.Experiences
            .Include(item => item.Media)
            .SingleOrDefaultAsync(item => item.Id == experienceId
                && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected), cancellationToken);
        if (experience is null)
        {
            return ExperienceMediaMutationOutcome.ExperienceUnavailable;
        }

        var mediaOfType = experience.Media.Count(media => media.MediaType == mediaType);
        if (mediaType == MediaType.Image && mediaOfType >= 8)
        {
            return ExperienceMediaMutationOutcome.ImageLimitReached;
        }
        if (mediaType == MediaType.Video && mediaOfType >= 2)
        {
            return ExperienceMediaMutationOutcome.VideoLimitReached;
        }

        experience.Media.Add(new ExperienceMedia
        {
            MediaType = mediaType,
            StorageKey = storageKey,
            DisplayOrder = experience.Media.Count == 0 ? 0 : experience.Media.Max(media => media.DisplayOrder) + 1,
            IsCover = mediaType == MediaType.Image && !experience.Media.Any(media => media.IsCover),
            CreatedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ExperienceMediaMutationOutcome.Success;
    }

    public async Task<(ExperienceMediaMutationOutcome Result, string? StorageKey)> DeleteAsync(string userId, int experienceId, int mediaId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var experience = await dbContext.Experiences
            .Include(item => item.Media)
            .SingleOrDefaultAsync(item => item.Id == experienceId
                && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected), cancellationToken);
        if (experience is null)
        {
            return (ExperienceMediaMutationOutcome.ExperienceUnavailable, null);
        }

        var media = experience.Media.SingleOrDefault(item => item.Id == mediaId);
        if (media is null)
        {
            return (ExperienceMediaMutationOutcome.MediaUnavailable, null);
        }

        var storageKey = media.StorageKey;
        var removedCover = media.IsCover;
        experience.Media.Remove(media);
        dbContext.ExperienceMedia.Remove(media);
        if (removedCover)
        {
            var replacement = experience.Media
                .Where(item => item.MediaType == MediaType.Image)
                .OrderBy(item => item.DisplayOrder)
                .ThenBy(item => item.Id)
                .FirstOrDefault();
            if (replacement is not null)
            {
                replacement.IsCover = true;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return (ExperienceMediaMutationOutcome.Success, storageKey);
    }

    public async Task<ExperienceMediaMutationOutcome> SetCoverAsync(string userId, int experienceId, int mediaId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var experience = await dbContext.Experiences
            .Include(item => item.Media)
            .SingleOrDefaultAsync(item => item.Id == experienceId
                && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected), cancellationToken);
        if (experience is null)
        {
            return ExperienceMediaMutationOutcome.ExperienceUnavailable;
        }

        var selected = experience.Media.SingleOrDefault(item => item.Id == mediaId);
        if (selected is null)
        {
            return ExperienceMediaMutationOutcome.MediaUnavailable;
        }
        if (selected.MediaType != MediaType.Image)
        {
            return ExperienceMediaMutationOutcome.CoverMustBeImage;
        }

        foreach (var image in experience.Media.Where(item => item.MediaType == MediaType.Image))
        {
            image.IsCover = image.Id == selected.Id;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ExperienceMediaMutationOutcome.Success;
    }

    public async Task<ExperienceMediaMutationOutcome> MoveAsync(string userId, int experienceId, int mediaId, int direction, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var experience = await dbContext.Experiences
            .Include(item => item.Media)
            .SingleOrDefaultAsync(item => item.Id == experienceId
                && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected), cancellationToken);
        if (experience is null)
        {
            return ExperienceMediaMutationOutcome.ExperienceUnavailable;
        }

        var ordered = experience.Media.OrderBy(item => item.DisplayOrder).ThenBy(item => item.Id).ToList();
        var currentIndex = ordered.FindIndex(item => item.Id == mediaId);
        var neighborIndex = currentIndex + direction;
        if (currentIndex < 0)
        {
            return ExperienceMediaMutationOutcome.MediaUnavailable;
        }
        if (neighborIndex < 0 || neighborIndex >= ordered.Count)
        {
            return ExperienceMediaMutationOutcome.InvalidOrder;
        }

        var current = ordered[currentIndex];
        var neighbor = ordered[neighborIndex];
        var temporaryOrder = ordered.Max(item => item.DisplayOrder) + 1;
        var currentOrder = current.DisplayOrder;
        var neighborOrder = neighbor.DisplayOrder;
        current.DisplayOrder = temporaryOrder;
        await dbContext.SaveChangesAsync(cancellationToken);
        neighbor.DisplayOrder = currentOrder;
        await dbContext.SaveChangesAsync(cancellationToken);
        current.DisplayOrder = neighborOrder;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ExperienceMediaMutationOutcome.Success;
    }

    private static string GetContentType(string storageKey, MediaType mediaType)
    {
        var extension = Path.GetExtension(storageKey).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".mp4" => "video/mp4",
            _ => mediaType == MediaType.Image ? "image/jpeg" : "video/mp4"
        };
    }
}
