using DiscoverEgypt.Application.DTOs.Reviews;
using DiscoverEgypt.Application.Responses.Reviews;
using MediatR;

namespace DiscoverEgypt.Application.Features.Reviews.Requests.Commands
{
    public sealed record CreateTouristReviewCommand(string UserId, CreateTouristReviewDto Form)
        : IRequest<TouristReviewCreateResponse>;
}
