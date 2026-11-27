using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Responses.Dashboard
{
    public sealed record HostDashboardExperienceStatusResponse(ExperienceStatus Status, int Count);
}
