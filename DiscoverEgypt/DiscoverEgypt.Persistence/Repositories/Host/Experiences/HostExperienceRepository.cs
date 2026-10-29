using System.Data;
using AutoMapper;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using Microsoft.EntityFrameworkCore;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Persistence.Repositories.Host.Experiences;

public sealed class HostExperienceRepository(DiscoverEgyptDbContext dbContext, IMapper mapper) : IHostExperienceRepository
{
    public async Task<IReadOnlyList<HostExperienceListItemDto>> GetForHostAsync(
        string userId,
        ExperienceStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Experiences
            .AsNoTracking()
            .Where(experience => experience.HostProfile.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(experience => experience.Status == status.Value);
        }

        return await query
            .OrderByDescending(experience => experience.UpdatedAt)
            .ThenByDescending(experience => experience.Id)
            .Select(experience => new HostExperienceListItemDto(
                experience.Id,
                experience.Title,
                experience.Description,
                experience.PricePerPerson,
                experience.DurationMinutes,
                experience.ExperienceType.Name,
                experience.CulturalArea.Name,
                experience.Status,
                experience.RejectionReason,
                experience.CreatedAt,
                experience.Media.Where(media => media.IsCover).Select(media => (int?)media.Id).FirstOrDefault()))
            .ToListAsync(cancellationToken);
    }

    public async Task<HostExperienceCreateOptionsDto> GetCreateOptionsAsync(CancellationToken cancellationToken = default)
    {
        var experienceTypes = await dbContext.ExperienceTypes
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new HostExperienceLookupDto(item.Id, item.Name))
            .ToListAsync(cancellationToken);
        var culturalAreas = await dbContext.CulturalAreas
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new HostExperienceLookupDto(item.Id, item.Name))
            .ToListAsync(cancellationToken);
        var interests = await dbContext.Interests
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new HostExperienceLookupDto(item.Id, item.Name))
            .ToListAsync(cancellationToken);
        var languages = await dbContext.Languages
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new HostExperienceLookupDto(item.Id, item.Name))
            .ToListAsync(cancellationToken);

        return new HostExperienceCreateOptionsDto(experienceTypes, culturalAreas, interests, languages);
    }

    public async Task<HostExperienceDraftEditorDto?> GetEditableDraftAsync(string userId, int experienceId, CancellationToken cancellationToken = default)
    {
        var experience = await dbContext.Experiences.AsNoTracking()
            .Include(item => item.ExperienceInterests)
            .Include(item => item.ExperienceLanguages)
            .SingleOrDefaultAsync(item => item.Id == experienceId && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected), cancellationToken);
        if (experience is null) return null;
        return mapper.Map<HostExperienceDraftEditorDto>(experience);
    }

    public async Task<int?> CreateDraftAsync(
        string userId,
        CreateHostExperienceDraftDto request,
        CancellationToken cancellationToken = default)
    {
        var hostProfileId = await dbContext.HostProfiles
            .Where(profile => profile.UserId == userId)
            .Select(profile => (int?)profile.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (!hostProfileId.HasValue
            || !await dbContext.ExperienceTypes.AnyAsync(item => item.Id == request.ExperienceTypeId, cancellationToken)
            || !await dbContext.CulturalAreas.AnyAsync(item => item.Id == request.CulturalAreaId, cancellationToken))
        {
            return null;
        }

        var interestIds = request.InterestIds.Distinct().ToArray();
        var languageIds = request.LanguageIds.Distinct().ToArray();
        if (interestIds.Length == 0
            || languageIds.Length == 0
            || await dbContext.Interests.CountAsync(item => interestIds.Contains(item.Id), cancellationToken) != interestIds.Length
            || await dbContext.Languages.CountAsync(item => languageIds.Contains(item.Id), cancellationToken) != languageIds.Length)
        {
            return null;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var experience = new Experience
        {
            HostProfileId = hostProfileId.Value,
            ExperienceTypeId = request.ExperienceTypeId,
            CulturalAreaId = request.CulturalAreaId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            CulturalStory = request.CulturalStory.Trim(),
            PricePerPerson = request.PricePerPerson,
            DurationMinutes = request.DurationMinutes,
            MaxGroupSize = request.MaxGroupSize,
            MeetingPointArea = request.MeetingPointArea.Trim(),
            ExactMeetingPoint = request.ExactMeetingPoint.Trim(),
            Status = ExperienceStatus.Draft,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Experiences.Add(experience);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.ExperienceInterests.AddRange(interestIds.Select(interestId => new ExperienceInterest
        {
            ExperienceId = experience.Id,
            InterestId = interestId
        }));
        dbContext.ExperienceLanguages.AddRange(languageIds.Select(languageId => new ExperienceLanguage
        {
            ExperienceId = experience.Id,
            LanguageId = languageId
        }));
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return experience.Id;
    }

    public async Task<bool> UpdateDraftAsync(string userId, int experienceId, CreateHostExperienceDraftDto request, CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var experience = await dbContext.Experiences
            .Include(item => item.ExperienceInterests)
            .Include(item => item.ExperienceLanguages)
            .SingleOrDefaultAsync(item => item.Id == experienceId && item.HostProfile.UserId == userId
                && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected), cancellationToken);
        if (experience is null
            || !await dbContext.ExperienceTypes.AnyAsync(item => item.Id == request.ExperienceTypeId, cancellationToken)
            || !await dbContext.CulturalAreas.AnyAsync(item => item.Id == request.CulturalAreaId, cancellationToken))
        {
            return false;
        }

        var interestIds = request.InterestIds.Distinct().ToArray();
        var languageIds = request.LanguageIds.Distinct().ToArray();
        if (interestIds.Length == 0 || languageIds.Length == 0
            || await dbContext.Interests.CountAsync(item => interestIds.Contains(item.Id), cancellationToken) != interestIds.Length
            || await dbContext.Languages.CountAsync(item => languageIds.Contains(item.Id), cancellationToken) != languageIds.Length)
        {
            return false;
        }

        experience.ExperienceTypeId = request.ExperienceTypeId;
        experience.CulturalAreaId = request.CulturalAreaId;
        experience.Title = request.Title.Trim();
        experience.Description = request.Description.Trim();
        experience.CulturalStory = request.CulturalStory.Trim();
        experience.PricePerPerson = request.PricePerPerson;
        experience.DurationMinutes = request.DurationMinutes;
        experience.MaxGroupSize = request.MaxGroupSize;
        experience.MeetingPointArea = request.MeetingPointArea.Trim();
        experience.ExactMeetingPoint = request.ExactMeetingPoint.Trim();
        experience.UpdatedAt = DateTime.UtcNow;
        var requestedInterests = interestIds.ToHashSet();
        var requestedLanguages = languageIds.ToHashSet();
        dbContext.ExperienceInterests.RemoveRange(experience.ExperienceInterests.Where(link => !requestedInterests.Contains(link.InterestId)));
        dbContext.ExperienceLanguages.RemoveRange(experience.ExperienceLanguages.Where(link => !requestedLanguages.Contains(link.LanguageId)));
        foreach (var interestId in requestedInterests.Except(experience.ExperienceInterests.Select(link => link.InterestId)))
            experience.ExperienceInterests.Add(new ExperienceInterest { ExperienceId = experience.Id, InterestId = interestId });
        foreach (var languageId in requestedLanguages.Except(experience.ExperienceLanguages.Select(link => link.LanguageId)))
            experience.ExperienceLanguages.Add(new ExperienceLanguage { ExperienceId = experience.Id, LanguageId = languageId });
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}
