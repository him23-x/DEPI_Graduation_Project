using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Bookings;
using DiscoverEgypt.Domain.Enums.Payments;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Services
{
    public sealed class GuideBookingService
    {
        public const decimal PlatformFeeRate = 0.15m;
        private static readonly TimeZoneInfo CairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "Egypt Standard Time" : "Africa/Cairo");

        private readonly IGuideBookingRepository _repository;
        private readonly TimeProvider _timeProvider;
        private readonly ITouristSanctionRepository _sanctionRepository;

        public GuideBookingService(
            IGuideBookingRepository repository,
            TimeProvider timeProvider,
            ITouristSanctionRepository sanctionRepository)
        {
            _repository = repository;
            _timeProvider = timeProvider;
            _sanctionRepository = sanctionRepository;
        }

        public async Task<GuideBookingFormResponse?> GetFormAsync(int guideProfileId, CancellationToken cancellationToken)
        {
            var guide = await GetGuideAsync(guideProfileId, cancellationToken);
            if (guide is null) return null;

            var minimumLocal = TimeZoneInfo.ConvertTimeFromUtc(
                _timeProvider.GetUtcNow().UtcDateTime.AddHours(72), CairoTimeZone);
            var form = new GuideBookingInputDto { GuideProfileId = guideProfileId, GuideHours = 1 };
            return new GuideBookingFormResponse(guide, form, DateTime.SpecifyKind(minimumLocal, DateTimeKind.Unspecified));
        }

        public async Task<GuideBookingReviewResponse> PreviewAsync(GuideBookingInputDto form, CancellationToken cancellationToken)
        {
            var evaluation = await EvaluateAsync(form, cancellationToken);
            return ToReviewResponse(evaluation);
        }

        public async Task<int> CreateAsync(string userId, GuideBookingInputDto form, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
            if (await _sanctionRepository.HasActiveTouristBanAsync(userId, nowUtc, cancellationToken))
                throw new ValidationException("Your account has an active restriction and cannot create new bookings. Existing bookings and disputes remain available.");

            var evaluation = await EvaluateAsync(form, cancellationToken);
            var booking = new Booking
            {
                TouristUserId = userId,
                ProviderUserId = evaluation.Guide.ProviderUserId,
                BookingType = BookingType.Guide,
                GuideProfileId = evaluation.Guide.GuideProfileId,
                ScheduledStart = evaluation.ScheduledStartUtc,
                ScheduledEnd = evaluation.ScheduledEndUtc,
                GuideHours = evaluation.GuideHours,
                Destination = evaluation.Destination,
                HourlyRateSnapshot = evaluation.Guide.HourlyRate,
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

        private async Task<GuideBookingEvaluation> EvaluateAsync(GuideBookingInputDto? form, CancellationToken cancellationToken)
        {
            if (form is null) throw new ValidationException("Booking details are required.");
            if (form.GuideProfileId <= 0) throw new ValidationException("Choose a valid guide.");
            if (!form.GuideHours.HasValue || form.GuideHours <= 0)
                throw new ValidationException("Choose at least one hour.");
            var destination = form.Destination?.Trim();
            if (string.IsNullOrWhiteSpace(destination)) throw new ValidationException("Enter the destination for your guide booking.");
            if (destination.Length > 300) throw new ValidationException("Destination must be 300 characters or fewer.");
            if (!form.ScheduledStartLocal.HasValue) throw new ValidationException("Choose a start date and time.");

            var guide = await GetGuideAsync(form.GuideProfileId, cancellationToken);
            if (guide is null) throw new ValidationException("This guide is no longer available for booking.");
            if (guide.HourlyRate <= 0) throw new ValidationException("This guide is not currently available for booking.");

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
                throw new ValidationException("Guide bookings must start at least 72 hours from now.");

            var endUtc = startUtc.AddHours(form.GuideHours.Value);
            if (await _repository.HasConfirmedOverlapAsync(guide.ProviderUserId, startUtc, endUtc, cancellationToken))
                throw new ValidationException("The guide already has a confirmed booking during this time.");

            var serviceAmount = decimal.Round(guide.HourlyRate * form.GuideHours.Value, 2, MidpointRounding.AwayFromZero);
            var platformFeeAmount = decimal.Round(serviceAmount * PlatformFeeRate, 2, MidpointRounding.AwayFromZero);
            return new GuideBookingEvaluation(
                guide,
                form.GuideHours.Value,
                destination,
                startUtc,
                endUtc,
                TimeZoneInfo.ConvertTimeFromUtc(startUtc, CairoTimeZone),
                TimeZoneInfo.ConvertTimeFromUtc(endUtc, CairoTimeZone),
                serviceAmount,
                platformFeeAmount,
                serviceAmount + platformFeeAmount);
        }

        private Task<GuideBookingDataDto?> GetGuideAsync(int guideProfileId, CancellationToken cancellationToken)
        {
            var todayUtc = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
            return _repository.GetApprovedGuideAsync(guideProfileId, todayUtc, cancellationToken);
        }

        private static GuideBookingReviewResponse ToReviewResponse(GuideBookingEvaluation evaluation) => new(
            evaluation.Guide.GuideProfileId,
            evaluation.Guide.GuideName,
            evaluation.Guide.ProfileImageKey,
            evaluation.Guide.Languages,
            evaluation.Guide.CulturalAreas,
            DateTime.SpecifyKind(evaluation.ScheduledStartLocal, DateTimeKind.Unspecified),
            DateTime.SpecifyKind(evaluation.ScheduledEndLocal, DateTimeKind.Unspecified),
            evaluation.GuideHours,
            evaluation.Destination,
            evaluation.Guide.HourlyRate,
            evaluation.ServiceAmount,
            PlatformFeeRate,
            evaluation.PlatformFeeAmount,
            evaluation.TotalAmount);

        private sealed record GuideBookingEvaluation(
            GuideBookingDataDto Guide,
            int GuideHours,
            string Destination,
            DateTime ScheduledStartUtc,
            DateTime ScheduledEndUtc,
            DateTime ScheduledStartLocal,
            DateTime ScheduledEndLocal,
            decimal ServiceAmount,
            decimal PlatformFeeAmount,
            decimal TotalAmount);
    }
}
