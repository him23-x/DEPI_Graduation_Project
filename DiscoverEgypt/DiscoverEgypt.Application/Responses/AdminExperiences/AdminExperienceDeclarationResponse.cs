using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Responses.AdminExperiences
{
    public sealed record AdminExperienceDeclarationResponse(
        DeclarationType Type,
        string Text,
        DateTime AcceptedAt);
}
