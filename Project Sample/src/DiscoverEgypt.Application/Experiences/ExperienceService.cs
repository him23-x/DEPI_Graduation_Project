using DiscoverEgypt.Application.Abstractions;

namespace DiscoverEgypt.Application.Experiences;

public class ExperienceService
{
    private readonly IExperienceRepository _experiences;

    public ExperienceService(IExperienceRepository experiences)
    {
        _experiences = experiences;
    }

    // Use case: "browse published experiences"
    public async Task<IReadOnlyList<ExperienceDto>> GetPublishedAsync()
    {
        var items = await _experiences.GetPublishedAsync();
        return items.Select(ExperienceDto.From).ToList();
    }
}
