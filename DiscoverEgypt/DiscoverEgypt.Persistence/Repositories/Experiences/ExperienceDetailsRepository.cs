using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.ExperienceDetails;
using DiscoverEgypt.Domain.Enums.Experiences;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence.Repositories.Experiences
{
    public sealed class ExperienceDetailsRepository : IExperienceDetailsRepository
    {
        private readonly DiscoverEgyptDbContext _dbContext;

        public ExperienceDetailsRepository(DiscoverEgyptDbContext dbContext) => _dbContext = dbContext;

        public Task<ExperienceDetailsDto?> GetPublishedDetailsAsync(int experienceId, CancellationToken cancellationToken = default) =>
            _dbContext.Experiences.AsNoTracking()
                .Where(experience => experience.Id == experienceId && experience.Status == ExperienceStatus.Published)
                .Select(experience => new ExperienceDetailsDto(
                    experience.Id,
                    experience.Title,
                    experience.ExperienceType.Name,
                    experience.CulturalArea.Name,
                    experience.MeetingPointArea,
                    experience.Description,
                    experience.CulturalStory,
                    experience.PricePerPerson,
                    experience.DurationMinutes,
                    experience.MaxGroupSize,
                    _dbContext.ApplicationUsers.Where(user => user.Id == experience.HostProfile.UserId)
                        .Select(user => user.FullName).FirstOrDefault() ?? string.Empty,
                    experience.HostProfile.Bio,
                    experience.ExperienceInterests.Select(link => link.Interest.Name).OrderBy(name => name).ToList(),
                    experience.ExperienceLanguages.Select(link => link.Language.Name).OrderBy(name => name).ToList(),
                    experience.Media.Where(media => media.MediaType == MediaType.Image)
                        .OrderByDescending(media => media.IsCover).ThenBy(media => media.DisplayOrder)
                        .Select(media => new ExperienceDetailsPhotoDto(media.Id, media.DisplayOrder)).ToList(),
                    experience.Bookings.Where(booking => booking.Review != null)
                        .Select(booking => (decimal?)booking.Review!.Rating).Average() ?? 0m,
                    experience.Bookings.Count(booking => booking.Review != null),
                    experience.Bookings.Where(booking => booking.Review != null)
                        .OrderByDescending(booking => booking.Review!.CreatedAt).Take(4)
                        .Select(booking => new ExperienceDetailsReviewDto(
                            _dbContext.ApplicationUsers.Where(user => user.Id == booking.TouristUserId)
                                .Select(user => user.FullName).FirstOrDefault() ?? "Traveler",
                            booking.Review!.Rating,
                            booking.Review.Comment,
                            booking.Review.CreatedAt)).ToList()))
                .FirstOrDefaultAsync(cancellationToken);

        public Task<string?> GetPublishedImageKeyAsync(int experienceId, int mediaId, CancellationToken cancellationToken = default) =>
            _dbContext.ExperienceMedia.AsNoTracking()
                .Where(media => media.Id == mediaId
                    && media.ExperienceId == experienceId
                    && media.MediaType == MediaType.Image
                    && media.Experience.Status == ExperienceStatus.Published)
                .Select(media => media.StorageKey)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
