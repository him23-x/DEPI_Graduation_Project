using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.DTOs.Dashboard;

public sealed record HostDashboardExperienceStatusDto(ExperienceStatus Status, int Count);
