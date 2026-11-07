using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.ViewModels.Dashboard;
using DiscoverEgypt.Application.Features.Dashboard.Requests.Queries;
using MediatR;
using AutoMapper;
using DiscoverEgypt.Domain.Enums.Experiences;
using DiscoverEgypt.Domain.Enums.Payments;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Dashboard.Handlers.Queries
{
    public sealed class GetHostDashboardQueryHandler : IRequestHandler<GetHostDashboardQuery, HostDashboardViewModel>
    {
        private readonly IHostDashboardRepository _repository;
        private readonly TimeProvider _timeProvider;
        private readonly IMapper _mapper;
        private readonly IValidator<GetHostDashboardQuery> _validator;

        public GetHostDashboardQueryHandler(IHostDashboardRepository repository, TimeProvider timeProvider, IMapper mapper, IValidator<GetHostDashboardQuery> validator)
        {
            _repository = repository;
            _timeProvider = timeProvider;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<HostDashboardViewModel> Handle(GetHostDashboardQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var experiences = await _repository.GetExperiencesAsync(request.UserId, cancellationToken);
            var statuses = Enum.GetValues<ExperienceStatus>()
                .Select(status => new HostDashboardExperienceStatusViewModel(status, experiences.Count(item => item.Status == status)))
                .ToArray();
            var pendingCount = await _repository.GetPendingRequestCountAsync(request.UserId, cancellationToken);
            var pending = await _repository.GetPendingRequestsAsync(request.UserId, 3, cancellationToken);
            var confirmedCount = await _repository.GetConfirmedBookingCountAsync(request.UserId, cancellationToken);
            var upcoming = await _repository.GetUpcomingConfirmedBookingsAsync(request.UserId, now, 3, cancellationToken);
            var payouts = await _repository.GetPayoutTotalsAsync(request.UserId, cancellationToken);
            var reviewCount = await _repository.GetReviewCountAsync(request.UserId, cancellationToken);
            var averageRating = await _repository.GetAverageRatingAsync(request.UserId, cancellationToken);
            var unreadCount = await _repository.GetUnreadNotificationCountAsync(request.UserId, cancellationToken);

            return new HostDashboardViewModel(
                statuses,
                pendingCount,
                _mapper.Map<IReadOnlyList<HostDashboardBookingViewModel>>(pending),
                confirmedCount,
                _mapper.Map<IReadOnlyList<HostDashboardBookingViewModel>>(upcoming),
                payouts.GetValueOrDefault(PayoutStatus.Pending),
                payouts.GetValueOrDefault(PayoutStatus.Paid),
                payouts.GetValueOrDefault(PayoutStatus.Cancelled),
                unreadCount,
                averageRating,
                reviewCount);
        }
    }
}

