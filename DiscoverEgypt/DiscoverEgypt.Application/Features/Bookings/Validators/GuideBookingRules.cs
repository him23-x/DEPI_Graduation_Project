using DiscoverEgypt.Application.DTOs.Bookings;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Validators
{
    internal static class GuideBookingRules
    {
        public static void Validate<T>(IRuleBuilderInitial<T, GuideBookingInputDto> formRule)
        {
            formRule.Must(form => form is not null && form.GuideProfileId > 0)
                .WithMessage("Choose a valid guide.");
            formRule.Must(form => form is not null && form.GuideHours.HasValue && form.GuideHours.Value > 0)
                .WithMessage("Choose at least one hour.");
            formRule.Must(form => form is not null && form.ScheduledStartLocal.HasValue && form.ScheduledStartLocal.Value != default)
                .WithMessage("Choose a start date and time.");
            formRule.Must(form => form is not null && !string.IsNullOrWhiteSpace(form.Destination))
                .WithMessage("Enter the destination for your guide booking.");
            formRule.Must(form => form is not null && (form.Destination?.Length ?? 0) <= 300)
                .WithMessage("Destination must be 300 characters or fewer.");
        }
    }
}
