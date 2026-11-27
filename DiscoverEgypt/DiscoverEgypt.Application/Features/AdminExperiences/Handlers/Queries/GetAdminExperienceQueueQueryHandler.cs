using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.AdminExperiences.Requests.Queries;
using DiscoverEgypt.Application.Responses.AdminExperiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminExperiences.Handlers.Queries
{
    public sealed class GetAdminExperienceQueueQueryHandler
        : IRequestHandler<GetAdminExperienceQueueQuery, IReadOnlyList<AdminExperienceQueueItemResponse>>
    {
        private readonly IAdminExperienceReviewRepository _repository;

        public GetAdminExperienceQueueQueryHandler(IAdminExperienceReviewRepository repository) =>
            _repository = repository;

        public Task<IReadOnlyList<AdminExperienceQueueItemResponse>> Handle(
            GetAdminExperienceQueueQuery request,
            CancellationToken cancellationToken) =>
            _repository.GetForManagementAsync(cancellationToken);
    }
}
