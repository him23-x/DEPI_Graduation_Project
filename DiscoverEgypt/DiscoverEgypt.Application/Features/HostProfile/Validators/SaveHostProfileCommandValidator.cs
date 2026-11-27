using DiscoverEgypt.Application.Features.HostProfile.Requests.Commands;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.HostProfile.Validators
{
    public sealed class SaveHostProfileCommandValidator : AbstractValidator<SaveHostProfileCommand>
    {
        public SaveHostProfileCommandValidator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.Profile).NotNull();
            When(command => command.Profile is not null, () =>
            {
                RuleFor(command => command.Profile.HostType).IsInEnum();
                RuleFor(command => command.Profile.Bio)
                    .Must(bio => !string.IsNullOrWhiteSpace(bio))
                    .WithMessage("Tell guests a little about yourself.")
                    .MaximumLength(2000);
            });

            When(command => command.ImageContent is not null, () =>
            {
                RuleFor(command => command.ImageLength).InclusiveBetween(1, 20 * 1024 * 1024);
                RuleFor(command => command.ImageExtension)
                    .Must(extension => extension is ".jpg" or ".jpeg" or ".png" or ".webp")
                    .WithMessage("Choose a JPG, PNG, or WebP image.");
                RuleFor(command => command.ImageContentType)
                    .Must((command, contentType) => contentType == command.ImageExtension switch
                    {
                        ".jpg" or ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".webp" => "image/webp",
                        _ => string.Empty
                    })
                    .WithMessage("The image type is not supported.");
            });
        }
    }
}
