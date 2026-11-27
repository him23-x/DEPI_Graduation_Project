using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Application.Features.Disputes.Requests.Queries;
using MediatR;
using AutoMapper;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Disputes.Handlers.Queries
{
    public sealed class GetHostDisputesQueryHandler
        : IRequestHandler<GetHostDisputesQuery, HostDisputesIndexDto>
    {
        private readonly IHostDisputesRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<GetHostDisputesQuery> _validator;

        public GetHostDisputesQueryHandler(IHostDisputesRepository repository, IMapper mapper, IValidator<GetHostDisputesQuery> validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<HostDisputesIndexDto> Handle(GetHostDisputesQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var disputes = await _repository.GetForHostAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
            var tourists = await _repository.GetTouristsAsync(disputes.Select(dispute => dispute.Booking.TouristUserId), cancellationToken);
            var items = _mapper.Map<IReadOnlyList<HostDisputeListItemDto>>(disputes).Select((item, index) =>
            {
                var dispute = disputes[index];
                var tourist = tourists.GetValueOrDefault(dispute.Booking.TouristUserId);
                return item with
                {
                    GuestName = !string.IsNullOrWhiteSpace(tourist?.FullName) ? tourist.FullName : tourist?.UserName ?? "Discover Egypt guest",
                    IsPayoutHeld = dispute.Status is Domain.Enums.Disputes.DisputeStatus.Open or Domain.Enums.Disputes.DisputeStatus.UnderReview
                        && dispute.Booking.Payout?.Status == Domain.Enums.Payments.PayoutStatus.Pending
                };
            }).ToArray();
            var totalCount = await _repository.GetCountAsync(request.UserId, cancellationToken);
            return new HostDisputesIndexDto(items, request.Page, request.PageSize, totalCount);
        }
    }
}

