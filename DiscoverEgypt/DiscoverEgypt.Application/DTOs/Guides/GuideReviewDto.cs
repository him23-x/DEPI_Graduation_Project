namespace DiscoverEgypt.Application.DTOs.Guides
{
    public sealed record GuideReviewDto(string TouristName, int Rating, string? Comment, DateTime CreatedAt);
}
