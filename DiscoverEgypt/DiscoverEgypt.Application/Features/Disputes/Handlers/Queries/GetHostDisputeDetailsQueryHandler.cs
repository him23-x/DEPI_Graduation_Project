using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using MediatR;
using AutoMapper;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Disputes.Handlers.Queries
{
    public sealed class GetHostDisputeDetailsQueryHandler
        : IRequestHandler<GetHostDisputeDetailsQuery, HostDisputeDetailsDto?>
    {
        private readonly IHostDisputesRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<GetHostDisputeDetailsQuery> _validator;

        public GetHostDisputeDetailsQueryHandler(IHostDisputesRepository repository, IMapper mapper, IValidator<GetHostDisputeDetailsQuery> validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<HostDisputeDetailsDto?> Handle(GetHostDisputeDetailsQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            if (request.DisputeId <= 0) return null;
            var dispute = await _repository.GetDetailsAsync(request.UserId, request.DisputeId, cancellationToken);
            if (dispute is null) return null;
            var tourist = (await _repository.GetTouristsAsync([dispute.Booking.TouristUserId], cancellationToken))
                .GetValueOrDefault(dispute.Booking.TouristUserId);
            var model = _mapper.Map<HostDisputeDetailsDto>(dispute);
            return model with
            {
                GuestName = !string.IsNullOrWhiteSpace(tourist?.FullName) ? tourist.FullName : tourist?.UserName ?? "Discover Egypt guest",
                IsPayoutHeld = dispute.Status is Domain.Enums.Disputes.DisputeStatus.Open or Domain.Enums.Disputes.DisputeStatus.UnderReview
                    && dispute.Booking.Payout?.Status == Domain.Enums.Payments.PayoutStatus.Pending
            };
        }
    }
}

