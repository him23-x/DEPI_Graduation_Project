
using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Policies;

public sealed record ExperienceDeclarationDefinition(DeclarationType Type, string Text);
