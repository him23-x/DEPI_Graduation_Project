using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Responses.Earnings;
using DiscoverEgypt.Application.Features.Earnings.Requests.Queries;
using MediatR;
using AutoMapper;
using DiscoverEgypt.Domain.Enums.Payments;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Earnings.Handlers.Queries
{
    public sealed class GetHostEarningsQueryHandler
        : IRequestHandler<GetHostEarningsQuery, HostEarningsResponse>
    {
        private readonly IHostEarningsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<GetHostEarningsQuery> _validator;

        public GetHostEarningsQueryHandler(IHostEarningsRepository repository, IMapper mapper, IValidator<GetHostEarningsQuery> validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<HostEarningsResponse> Handle(GetHostEarningsQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var payouts = await _repository.GetForHostAsync(
                request.UserId,
                request.Status,
                request.Page,
                request.PageSize,
                cancellationToken);
            var totals = await _repository.GetTotalsAsync(request.UserId, cancellationToken);
            var count = await _repository.GetCountAsync(request.UserId, request.Status, cancellationToken);
            return new HostEarningsResponse(
                _mapper.Map<IReadOnlyList<HostPayoutItemResponse>>(payouts),
                totals.GetValueOrDefault(PayoutStatus.Pending),
                totals.GetValueOrDefault(PayoutStatus.Paid),
                totals.GetValueOrDefault(PayoutStatus.Cancelled),
                count,
                request.Page,
                request.PageSize)
            {
                SelectedStatus = request.Status
            };
        }
    }
}

