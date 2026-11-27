using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Guides;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Verification;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Guides
{
    public sealed class ApprovedGuideRepository : IApprovedGuideRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public ApprovedGuideRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public async Task<IReadOnlyList<GuideProfileSummaryDto>> GetPageAsync(
            string search, int? languageId, int? culturalAreaId, int skip, int take,
            CancellationToken cancellationToken = default)
        {
            var orderedProfiles =
                from guide in Filtered(search, languageId, culturalAreaId)
                join user in _dbContext.ApplicationUsers.AsNoTracking() on guide.UserId equals user.Id
                orderby user.FullName, guide.Id
                select guide;

            return await Summaries(orderedProfiles)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountAsync(string search, int? languageId, int? culturalAreaId, CancellationToken cancellationToken = default) =>
            Filtered(search, languageId, culturalAreaId).CountAsync(cancellationToken);

        public async Task<IReadOnlyList<GuideDirectoryOptionDto>> GetLanguagesAsync(CancellationToken cancellationToken = default)
        {
            var approvedGuideIds = PublicApprovedProfiles().Select(guide => guide.Id);
            return await _dbContext.Languages.AsNoTracking()
                .Where(language => _dbContext.GuideLanguages.Any(link => link.LanguageId == language.Id && approvedGuideIds.Contains(link.GuideProfileId)))
                .OrderBy(language => language.Name)
                .Select(language => new GuideDirectoryOptionDto(language.Id, language.Name))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<GuideDirectoryOptionDto>> GetCulturalAreasAsync(CancellationToken cancellationToken = default)
        {
            var approvedGuideIds = PublicApprovedProfiles().Select(guide => guide.Id);
            return await _dbContext.CulturalAreas.AsNoTracking()
                .Where(area => _dbContext.GuideCulturalAreas.Any(link => link.CulturalAreaId == area.Id && approvedGuideIds.Contains(link.GuideProfileId)))
                .OrderBy(area => area.Name)
                .Select(area => new GuideDirectoryOptionDto(area.Id, area.Name))
                .ToListAsync(cancellationToken);
        }

        public async Task<GuideProfileDetailsDto?> GetDetailsAsync(int guideProfileId, CancellationToken cancellationToken = default)
        {
            var profile = await Summaries(PublicApprovedProfiles().Where(guide => guide.Id == guideProfileId))
                .FirstOrDefaultAsync(cancellationToken);
            if (profile is null) return null;

            var reviews = await PublicApprovedProfiles().Where(guide => guide.Id == guideProfileId)
                .SelectMany(guide => guide.Bookings.Where(booking => booking.Review != null))
                .OrderByDescending(booking => booking.Review!.CreatedAt)
                .Take(6)
                .Select(booking => new GuideReviewDto(
                    _dbContext.ApplicationUsers.Where(user => user.Id == booking.TouristUserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? "Traveler",
                    booking.Review!.Rating,
                    booking.Review.Comment,
                    booking.Review.CreatedAt))
                .ToListAsync(cancellationToken);

            return new GuideProfileDetailsDto(profile, reviews);
        }

        public Task<string?> GetApprovedProfileImageKeyAsync(int guideProfileId, CancellationToken cancellationToken = default) =>
            PublicApprovedProfiles().Where(guide => guide.Id == guideProfileId)
                .Select(guide => guide.ProfileImageKey)
                .FirstOrDefaultAsync(cancellationToken);

        private IQueryable<GuideProfile> PublicApprovedProfiles()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return _dbContext.GuideProfiles.AsNoTracking()
                .Where(guide => guide.VerificationRequests
                    .OrderByDescending(request => request.SubmittedAt)
                    .ThenByDescending(request => request.Id)
                    .Select(request => request.Status)
                    .FirstOrDefault() == VerificationStatus.Approved)
                .Where(guide => guide.VerificationRequests
                    .OrderByDescending(request => request.SubmittedAt)
                    .ThenByDescending(request => request.Id)
                    .Select(request => (DateOnly?)request.LicenseExpiryDate)
                    .FirstOrDefault() >= today);
        }

        private IQueryable<GuideProfile> Filtered(string search, int? languageId, int? culturalAreaId)
        {
            var query = PublicApprovedProfiles();
            if (languageId.HasValue)
                query = query.Where(guide => guide.GuideLanguages.Any(link => link.LanguageId == languageId.Value));
            if (culturalAreaId.HasValue)
                query = query.Where(guide => guide.GuideCulturalAreas.Any(link => link.CulturalAreaId == culturalAreaId.Value));
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(guide => guide.Bio.Contains(search)
                    || _dbContext.ApplicationUsers.Any(user => user.Id == guide.UserId && user.FullName.Contains(search))
                    || guide.GuideLanguages.Any(link => link.Language.Name.Contains(search))
                    || guide.GuideCulturalAreas.Any(link => link.CulturalArea.Name.Contains(search)));
            }

            return query;
        }

        private IQueryable<GuideProfileSummaryDto> Summaries(IQueryable<GuideProfile> query) =>
            query.Select(guide => new GuideProfileSummaryDto(
                guide.Id,
                _dbContext.ApplicationUsers.Where(user => user.Id == guide.UserId)
                    .Select(user => user.FullName).FirstOrDefault() ?? string.Empty,
                guide.Bio,
                guide.HourlyRate,
                guide.ProfileImageKey,
                guide.GuideLanguages.Select(link => link.Language.Name).OrderBy(name => name).ToList(),
                guide.GuideCulturalAreas.Select(link => link.CulturalArea.Name).OrderBy(name => name).ToList(),
                guide.Bookings.Where(booking => booking.Review != null)
                    .Select(booking => (decimal?)booking.Review!.Rating).Average() ?? 0m,
                guide.Bookings.Count(booking => booking.Review != null)));
    }
}
