using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Responses.Reviews;
using DiscoverEgypt.Application.Features.Reviews.Requests.Queries;
using MediatR;
using AutoMapper;
using FluentValidation;

namespace DiscoverEgypt.Application.Features.Reviews.Handlers.Queries
{
    public sealed class GetHostReviewsQueryHandler
        : IRequestHandler<GetHostReviewsQuery, HostReviewsResponse>
    {
        private readonly IHostReviewsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<GetHostReviewsQuery> _validator;

        public GetHostReviewsQueryHandler(IHostReviewsRepository repository, IMapper mapper, IValidator<GetHostReviewsQuery> validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<HostReviewsResponse> Handle(GetHostReviewsQuery request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var reviews = await _repository.GetForHostAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
            var tourists = await _repository.GetTouristsAsync(reviews.Select(review => review.Booking.TouristUserId), cancellationToken);
            var items = _mapper.Map<IReadOnlyList<HostReviewItemResponse>>(reviews).Select((item, index) =>
            {
                var tourist = tourists.GetValueOrDefault(reviews[index].Booking.TouristUserId);
                return item with { GuestName = !string.IsNullOrWhiteSpace(tourist?.FullName) ? tourist.FullName : tourist?.UserName ?? "Discover Egypt guest" };
            }).ToArray();
            var count = await _repository.GetCountAsync(request.UserId, cancellationToken);
            var average = await _repository.GetAverageRatingAsync(request.UserId, cancellationToken);
            return new HostReviewsResponse(items, average, count, request.Page, request.PageSize, count);
        }
    }
}

