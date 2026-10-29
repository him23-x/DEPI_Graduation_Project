using DiscoverEgypt.Application.DTOs.HostExperiences;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.HostExperiences.Validators.DTOs;

public sealed class CreateExperienceDtoValidator : AbstractValidator<CreateExperienceDto>
{
    public CreateExperienceDtoValidator()
    {
        RuleFor(x => x.ExperienceTypeId).GreaterThan(0).WithMessage("Choose an experience type.");
        RuleFor(x => x.CulturalAreaId).GreaterThan(0).WithMessage("Choose a cultural area.");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.CulturalStory).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.PricePerPerson).InclusiveBetween(0.01m, 9999999999999999.99m).WithMessage("Enter a price greater than zero.");
        RuleFor(x => x.DurationMinutes).GreaterThan(0).WithMessage("Duration must be greater than zero.");
        RuleFor(x => x.MaxGroupSize).GreaterThan(0).WithMessage("Group size must be at least one.");
        RuleFor(x => x.MeetingPointArea).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ExactMeetingPoint).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.InterestIds).NotEmpty().WithMessage("Choose at least one interest.");
        RuleFor(x => x.LanguageIds).NotEmpty().WithMessage("Choose at least one language.");
    }
}
