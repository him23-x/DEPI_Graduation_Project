using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.ViewModels.Bookings;
using DiscoverEgypt.Application.Features.Bookings.Requests.Queries;
using DiscoverEgypt.Application.Features.Bookings.Options;
using DiscoverEgypt.Application.Features.Bookings.Policies;
using Microsoft.Extensions.Options;
using MediatR;
using AutoMapper;
using DiscoverEgypt.Domain.Enums.Disputes;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Bookings.Handlers.Queries
{
    public sealed class GetHostBookingDetailsQueryHandler : IRequestHandler<GetHostBookingDetailsQuery, BookingDetailsViewModel?>
    {
        private readonly IHostBookingRepository _repository;
        private readonly IOptions<BookingLifecycleOptions> _lifecycleOptions;
        private readonly IMapper _mapper;
        private readonly IValidator<GetHostBookingDetailsQuery> _validator;

        public GetHostBookingDetailsQueryHandler(
            IHostBookingRepository repository,
            IOptions<BookingLifecycleOptions> lifecycleOptions,
            IMapper mapper,
            IValidator<GetHostBookingDetailsQuery> validator)
        {
            _repository = repository;
            _lifecycleOptions = lifecycleOptions;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<BookingDetailsViewModel?> Handle(GetHostBookingDetailsQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var bookingEntity = await _repository.GetDetailsAsync(request.UserId, request.BookingId, cancellationToken);
            if (bookingEntity is null) return null;
            var tourist = (await _repository.GetTouristsAsync([bookingEntity.TouristUserId], cancellationToken)).GetValueOrDefault(bookingEntity.TouristUserId);
            var now = DateTime.UtcNow;
            var isSuspended = await _repository.IsProviderSuspendedAsync(request.UserId, now, cancellationToken);
            var hasOverlap = bookingEntity.Status == DiscoverEgypt.Domain.Enums.Bookings.BookingStatus.Pending
                && await _repository.HasConfirmedOverlapAsync(request.UserId, bookingEntity.ScheduledStart, bookingEntity.ScheduledEnd, bookingEntity.Id, cancellationToken);
            var booking = _mapper.Map<BookingDetailsViewModel>(bookingEntity) with
            {
                TouristName = !string.IsNullOrWhiteSpace(tourist?.FullName) ? tourist.FullName : tourist?.UserName ?? "Discover Egypt guest",
                TouristEmail = tourist?.Email ?? string.Empty,
                IsProviderSuspended = isSuspended,
                HasOverlap = hasOverlap,
                HasOpenDispute = bookingEntity.Disputes.Any(dispute => dispute.Status is DisputeStatus.Open or DisputeStatus.UnderReview),
                HasPayout = bookingEntity.Payout is not null
            };

            var acceptFailure = HostBookingPolicy.GetAcceptFailure(
                booking.Status,
                booking.PaymentStatus,
                booking.PaidAt,
                booking.IsProviderSuspended,
                booking.HasOverlap,
                booking.HasPayout,
                now,
                _lifecycleOptions.Value.ProviderResponseTimeout);

            return booking with
            {
                ResponseDeadline = HostBookingPolicy.GetResponseDeadline(
                    booking.PaidAt,
                    _lifecycleOptions.Value.ProviderResponseTimeout),
                CanAccept = acceptFailure is null,
                CanReject = HostBookingPolicy.GetRejectFailure(
                    booking.Status,
                    booking.PaymentStatus,
                    booking.ServiceAmount) is null,
                CanComplete = HostBookingPolicy.GetCompleteFailure(
                    booking.Status,
                    booking.ScheduledEnd,
                    booking.HasOpenDispute,
                    now) is null
            };
        }
    }
}

