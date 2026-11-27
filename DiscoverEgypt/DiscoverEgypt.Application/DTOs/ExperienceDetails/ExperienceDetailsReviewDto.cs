namespace DiscoverEgypt.Application.DTOs.ExperienceDetails
{
    public sealed record ExperienceDetailsReviewDto(string TouristName, int Rating, string? Comment, DateTime CreatedAt);
}
