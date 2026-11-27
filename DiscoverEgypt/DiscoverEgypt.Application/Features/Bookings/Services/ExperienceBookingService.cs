using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Services
{
    public sealed class ExperienceBookingService
    {
        public const decimal PlatformFeeRate = 0.15m;
        private static readonly TimeZoneInfo CairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "Egypt Standard Time" : "Africa/Cairo");

        private readonly IExperienceBookingRepository _repository;
        private readonly TimeProvider _timeProvider;
        private readonly ITouristSanctionRepository _sanctionRepository;

        public ExperienceBookingService(
            IExperienceBookingRepository repository,
            TimeProvider timeProvider,
            ITouristSanctionRepository sanctionRepository)
        {
            _repository = repository;
            _timeProvider = timeProvider;
            _sanctionRepository = sanctionRepository;
        }

        public async Task<ExperienceBookingFormResponse?> GetFormAsync(int experienceId, CancellationToken cancellationToken)
        {
            var experience = await _repository.GetPublishedExperienceAsync(experienceId, cancellationToken);
            if (experience is null) return null;

            var minimumUtc = _timeProvider.GetUtcNow().UtcDateTime.AddHours(72);
            var ticksPastMinute = minimumUtc.Ticks % TimeSpan.TicksPerMinute;
            if (ticksPastMinute != 0)
                minimumUtc = minimumUtc.AddTicks(TimeSpan.TicksPerMinute - ticksPastMinute);

            var minimumLocal = TimeZoneInfo.ConvertTimeFromUtc(minimumUtc, CairoTimeZone);
            return new ExperienceBookingFormResponse(
                experience,
                new ExperienceBookingInputDto { ExperienceId = experience.ExperienceId, NumberOfGuests = 1 },
                DateTime.SpecifyKind(minimumLocal, DateTimeKind.Unspecified));
        }

        public async Task<ExperienceBookingReviewResponse> PreviewAsync(
            ExperienceBookingInputDto form,
            CancellationToken cancellationToken)
        {
            var evaluation = await EvaluateAsync(form, cancellationToken);
            return ToReviewResponse(evaluation);
        }

        public async Task<int> CreateAsync(string userId, ExperienceBookingInputDto form, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
            if (await _sanctionRepository.HasActiveTouristBanAsync(userId, nowUtc, cancellationToken))
                throw new ValidationException("Your account has an active restriction and cannot create new bookings. Existing bookings and disputes remain available.");

            var evaluation = await EvaluateAsync(form, cancellationToken);

            var booking = new Booking
            {
                TouristUserId = userId,
                ProviderUserId = evaluation.Experience.ProviderUserId,
                BookingType = BookingType.Experience,
                ExperienceId = evaluation.Experience.ExperienceId,
                ScheduledStart = evaluation.ScheduledStartUtc,
                ScheduledEnd = evaluation.ScheduledEndUtc,
                NumberOfGuests = evaluation.NumberOfGuests,
                PricePerPersonSnapshot = evaluation.Experience.PricePerPerson,
                ServiceAmount = evaluation.ServiceAmount,
                PlatformFeeRate = PlatformFeeRate,
                PlatformFeeAmount = evaluation.PlatformFeeAmount,
                TotalAmount = evaluation.TotalAmount,
                Status = BookingStatus.Pending,
                CreatedAt = nowUtc,
                Payment = new Payment
                {
                    Amount = evaluation.TotalAmount,
                    Status = PaymentStatus.Pending,
                    CreatedAt = nowUtc
                }
            };

            return await _repository.CreateAsync(booking, cancellationToken);
        }

        private async Task<BookingEvaluation> EvaluateAsync(ExperienceBookingInputDto? form, CancellationToken cancellationToken)
        {
            if (form is null) throw new ValidationException("Booking details are required.");
            if (form.ExperienceId <= 0) throw new ValidationException("Choose a valid experience.");
            if (!form.NumberOfGuests.HasValue || form.NumberOfGuests <= 0)
                throw new ValidationException("Choose at least one guest.");
            if (!form.ScheduledStartLocal.HasValue)
                throw new ValidationException("Choose a start date and time.");

            var experience = await _repository.GetPublishedExperienceAsync(form.ExperienceId, cancellationToken);
            if (experience is null)
                throw new ValidationException("This experience is no longer available for booking.");
            if (form.NumberOfGuests.Value > experience.MaxGroupSize)
                throw new ValidationException($"This experience allows up to {experience.MaxGroupSize} guests.");

            var localStart = DateTime.SpecifyKind(form.ScheduledStartLocal.Value, DateTimeKind.Unspecified);
            if (CairoTimeZone.IsInvalidTime(localStart) || CairoTimeZone.IsAmbiguousTime(localStart))
                throw new ValidationException("Choose a different Cairo local time; this time is affected by a daylight-saving change.");

            DateTime startUtc;
            try
            {
                startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, CairoTimeZone);
            }
            catch (ArgumentException)
            {
                throw new ValidationException("Choose a valid Cairo local date and time.");
            }

            var minimumStartUtc = _timeProvider.GetUtcNow().UtcDateTime.AddHours(72);
            if (startUtc < minimumStartUtc)
                throw new ValidationException("Experience bookings must start at least 72 hours from now.");

            var endUtc = startUtc.AddMinutes(experience.DurationMinutes);
            if (await _repository.HasConfirmedOverlapAsync(
                    experience.ProviderUserId, startUtc, endUtc, cancellationToken))
                throw new ValidationException("The host already has a confirmed booking during this time.");

            var serviceAmount = decimal.Round(
                experience.PricePerPerson * form.NumberOfGuests.Value, 2, MidpointRounding.AwayFromZero);
            var platformFeeAmount = decimal.Round(
                serviceAmount * PlatformFeeRate, 2, MidpointRounding.AwayFromZero);
            var totalAmount = serviceAmount + platformFeeAmount;

            return new BookingEvaluation(
                experience,
                form.NumberOfGuests.Value,
                startUtc,
                endUtc,
                TimeZoneInfo.ConvertTimeFromUtc(startUtc, CairoTimeZone),
                TimeZoneInfo.ConvertTimeFromUtc(endUtc, CairoTimeZone),
                serviceAmount,
                platformFeeAmount,
                totalAmount);
        }

        private static ExperienceBookingReviewResponse ToReviewResponse(BookingEvaluation evaluation) => new(
            evaluation.Experience.ExperienceId,
            evaluation.Experience.Title,
            evaluation.Experience.ExperienceType,
            evaluation.Experience.CulturalArea,
            evaluation.Experience.MeetingPointArea,
            evaluation.Experience.HostName,
            DateTime.SpecifyKind(evaluation.ScheduledStartLocal, DateTimeKind.Unspecified),
            DateTime.SpecifyKind(evaluation.ScheduledEndLocal, DateTimeKind.Unspecified),
            evaluation.NumberOfGuests,
            evaluation.Experience.PricePerPerson,
            evaluation.ServiceAmount,
            PlatformFeeRate,
            evaluation.PlatformFeeAmount,
            evaluation.TotalAmount,
            evaluation.Experience.DurationMinutes);

        private sealed record BookingEvaluation(
            ExperienceBookingDataDto Experience,
            int NumberOfGuests,
            DateTime ScheduledStartUtc,
            DateTime ScheduledEndUtc,
            DateTime ScheduledStartLocal,
            DateTime ScheduledEndLocal,
            decimal ServiceAmount,
            decimal PlatformFeeAmount,
            decimal TotalAmount);
    }
}
