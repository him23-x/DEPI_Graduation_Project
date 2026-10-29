using DiscoverEgypt.Application.DTOs.HostExperiences;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators.Commands;

public sealed class CreateHostExperienceDraftDtoValidator : AbstractValidator<CreateHostExperienceDraftDto>
{
    public CreateHostExperienceDraftDtoValidator()
    {
        RuleFor(x => x.ExperienceTypeId).GreaterThan(0);
        RuleFor(x => x.CulturalAreaId).GreaterThan(0);
        RuleFor(x => x.Title).Must(value => value is not null && !string.IsNullOrWhiteSpace(value)).WithMessage("Title is required.")
            .Must(value => value is null || value.Trim().Length <= 200).WithMessage("Title cannot exceed 200 characters.");
        RuleFor(x => x.Description).Must(value => value is not null && !string.IsNullOrWhiteSpace(value)).WithMessage("Description is required.")
            .Must(value => value is null || value.Trim().Length <= 5000).WithMessage("Description cannot exceed 5000 characters.");
        RuleFor(x => x.CulturalStory).Must(value => value is not null && !string.IsNullOrWhiteSpace(value)).WithMessage("Cultural story is required.")
            .Must(value => value is null || value.Trim().Length <= 5000).WithMessage("Cultural story cannot exceed 5000 characters.");
        RuleFor(x => x.PricePerPerson).GreaterThan(0).LessThanOrEqualTo(9999999999999999.99m);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.MaxGroupSize).GreaterThan(0);
        RuleFor(x => x.MeetingPointArea).Must(value => value is not null && !string.IsNullOrWhiteSpace(value)).WithMessage("Meeting area is required.")
            .Must(value => value is null || value.Trim().Length <= 300).WithMessage("Meeting area cannot exceed 300 characters.");
        RuleFor(x => x.ExactMeetingPoint).Must(value => value is not null && !string.IsNullOrWhiteSpace(value)).WithMessage("Exact meeting point is required.")
            .Must(value => value is null || value.Trim().Length <= 1000).WithMessage("Exact meeting point cannot exceed 1000 characters.");
        RuleFor(x => x.InterestIds).NotNull().Must(ids => ids is { Count: > 0 }).WithMessage("Choose at least one interest.");
        RuleForEach(x => x.InterestIds).GreaterThan(0).When(x => x.InterestIds is not null);
        RuleFor(x => x.LanguageIds).NotNull().Must(ids => ids is { Count: > 0 }).WithMessage("Choose at least one language.");
        RuleForEach(x => x.LanguageIds).GreaterThan(0).When(x => x.LanguageIds is not null);
    }
}
