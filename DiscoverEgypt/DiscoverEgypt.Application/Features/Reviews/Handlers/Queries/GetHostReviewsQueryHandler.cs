using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.Reviews;
using DiscoverEgypt.Application.Features.Reviews.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.Reviews.Handlers.Queries;

public sealed class GetHostReviewsQueryHandler(IHostReviewsRepository repository)
    : IRequestHandler<GetHostReviewsQuery, HostReviewsPageDto>
{
    public Task<HostReviewsPageDto> Handle(GetHostReviewsQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
        return repository.GetForHostAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
    }
}
