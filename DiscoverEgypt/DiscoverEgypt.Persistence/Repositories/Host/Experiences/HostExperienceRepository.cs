using System.Data;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Persistence.Repositories.Host.Experiences
{
    public sealed class HostExperienceRepository : IHostExperienceRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;
        public HostExperienceRepository(DiscoverEgyptDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Experience>> GetForHostAsync(
            string userId,
            ExperienceStatus? status,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Experiences
                .AsNoTracking()
                .Where(experience => experience.HostProfile.UserId == userId);

            if (status.HasValue)
            {
                query = query.Where(experience => experience.Status == status.Value);
            }

            return await query
                .OrderByDescending(experience => experience.UpdatedAt)
                .ThenByDescending(experience => experience.Id)
                .Include(experience => experience.ExperienceType)
                .Include(experience => experience.CulturalArea)
                .Include(experience => experience.Media)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ExperienceType>> GetExperienceTypesAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.ExperienceTypes.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<CulturalArea>> GetCulturalAreasAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.CulturalAreas.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Interest>> GetInterestsAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.Interests.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Language>> GetLanguagesAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.Languages.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);

        public async Task<Experience?> GetEditableDraftAsync(string userId, int experienceId, CancellationToken cancellationToken = default)
        {
            var experience = await _dbContext.Experiences.AsNoTracking()
                .Include(item => item.ExperienceInterests)
                .Include(item => item.ExperienceLanguages)
                .Include(item => item.ExperienceType)
                .Include(item => item.CulturalArea)
                .Include(item => item.Media)
                .SingleOrDefaultAsync(item => item.Id == experienceId && item.HostProfile.UserId == userId
                    && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected), cancellationToken);
            return experience;
        }

        public async Task<int?> CreateDraftAsync(
            string userId,
            Experience draft,
            CancellationToken cancellationToken = default)
        {
            var hostProfileId = await _dbContext.HostProfiles
                .Where(profile => profile.UserId == userId)
                .Select(profile => (int?)profile.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (!hostProfileId.HasValue)
            {
                return null;
            }

            var interestIds = draft.ExperienceInterests.Select(item => item.InterestId).ToArray();
            var languageIds = draft.ExperienceLanguages.Select(item => item.LanguageId).ToArray();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            var now = DateTime.UtcNow;
            var experience = new Experience
            {
                HostProfileId = hostProfileId.Value,
                ExperienceTypeId = draft.ExperienceTypeId,
                CulturalAreaId = draft.CulturalAreaId,
                Title = draft.Title,
                Description = draft.Description,
                CulturalStory = draft.CulturalStory,
                PricePerPerson = draft.PricePerPerson,
                DurationMinutes = draft.DurationMinutes,
                MaxGroupSize = draft.MaxGroupSize,
                MeetingPointArea = draft.MeetingPointArea,
                ExactMeetingPoint = draft.ExactMeetingPoint,
                Status = ExperienceStatus.Draft,
                CreatedAt = now,
                UpdatedAt = now
            };

            _dbContext.Experiences.Add(experience);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _dbContext.ExperienceInterests.AddRange(interestIds.Select(interestId => new ExperienceInterest
            {
                ExperienceId = experience.Id,
                InterestId = interestId
            }));
            _dbContext.ExperienceLanguages.AddRange(languageIds.Select(languageId => new ExperienceLanguage
            {
                ExperienceId = experience.Id,
                LanguageId = languageId
            }));
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return experience.Id;
        }

        public async Task<bool> UpdateDraftAsync(string userId, int experienceId, Experience request, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var experience = await _dbContext.Experiences
                .Include(item => item.ExperienceInterests)
                .Include(item => item.ExperienceLanguages)
                .SingleOrDefaultAsync(item => item.Id == experienceId && item.HostProfile.UserId == userId
                    && (item.Status == ExperienceStatus.Draft || item.Status == ExperienceStatus.Rejected), cancellationToken);
            if (experience is null)
            {
                return false;
            }

            var interestIds = request.ExperienceInterests.Select(item => item.InterestId).ToArray();
            var languageIds = request.ExperienceLanguages.Select(item => item.LanguageId).ToArray();

            experience.ExperienceTypeId = request.ExperienceTypeId;
            experience.CulturalAreaId = request.CulturalAreaId;
            experience.Title = request.Title;
            experience.Description = request.Description;
            experience.CulturalStory = request.CulturalStory;
            experience.PricePerPerson = request.PricePerPerson;
            experience.DurationMinutes = request.DurationMinutes;
            experience.MaxGroupSize = request.MaxGroupSize;
            experience.MeetingPointArea = request.MeetingPointArea;
            experience.ExactMeetingPoint = request.ExactMeetingPoint;
            experience.UpdatedAt = DateTime.UtcNow;
            var requestedInterests = interestIds.ToHashSet();
            var requestedLanguages = languageIds.ToHashSet();
            _dbContext.ExperienceInterests.RemoveRange(experience.ExperienceInterests.Where(link => !requestedInterests.Contains(link.InterestId)));
            _dbContext.ExperienceLanguages.RemoveRange(experience.ExperienceLanguages.Where(link => !requestedLanguages.Contains(link.LanguageId)));
            foreach (var interestId in requestedInterests.Except(experience.ExperienceInterests.Select(link => link.InterestId)))
                experience.ExperienceInterests.Add(new ExperienceInterest { ExperienceId = experience.Id, InterestId = interestId });
            foreach (var languageId in requestedLanguages.Except(experience.ExperienceLanguages.Select(link => link.LanguageId)))
                experience.ExperienceLanguages.Add(new ExperienceLanguage { ExperienceId = experience.Id, LanguageId = languageId });
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
    }
}

