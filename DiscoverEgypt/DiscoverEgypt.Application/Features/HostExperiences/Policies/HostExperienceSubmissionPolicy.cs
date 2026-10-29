using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Enums.HostExperiences;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Policies;

public static class HostExperienceSubmissionPolicy
{
    public static EvidenceExperienceType Classify(string typeName)
    {
        var normalized = typeName.Trim();
        if (normalized.Equals("Food & Cooking", StringComparison.OrdinalIgnoreCase) || normalized.Equals("Food and Cooking", StringComparison.OrdinalIgnoreCase)) return EvidenceExperienceType.Food;
        if (normalized.Equals("Arts & Crafts", StringComparison.OrdinalIgnoreCase) || normalized.Equals("Arts and Crafts", StringComparison.OrdinalIgnoreCase)) return EvidenceExperienceType.Arts;
        if (normalized.Equals("Local Traditions & Daily Life", StringComparison.OrdinalIgnoreCase) || normalized.Equals("Local Traditions and Daily Life", StringComparison.OrdinalIgnoreCase)) return EvidenceExperienceType.LocalTraditions;
        return EvidenceExperienceType.Unsupported;
    }

    public static ExperienceDeclarationDefinition? GetDeclaration(EvidenceExperienceType type) => type switch
    {
        EvidenceExperienceType.Food => new(DeclarationType.FoodSafety,
            "I confirm that the food served during this experience is prepared hygienically, that ingredients and common allergens are accurately disclosed, and that safe food-handling practices are followed."),
        EvidenceExperienceType.Arts => new(DeclarationType.ToolsMaterialsSafety,
            "I confirm that I will use the tools and materials safely, explain relevant risks to guests, and provide appropriate supervision and protective measures."),
        EvidenceExperienceType.LocalTraditions => new(DeclarationType.PrivacyVenuePermission,
            "I confirm that I have the necessary permission to use the stated venue or route, and that I will respect participant privacy and obtain consent before photographing, recording, or sharing identifiable information."),
        _ => null
    };

}

