using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Queries;
using DiscoverEgypt.Application.Responses.GuideVerification;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminGuideVerification.Handlers.Queries
{
    public sealed class GetAdminGuideVerificationQueueQueryHandler
        : IRequestHandler<GetAdminGuideVerificationQueueQuery, IReadOnlyList<AdminGuideVerificationQueueItemResponse>>
    {
        private readonly IAdminGuideVerificationRepository _repository;

        public GetAdminGuideVerificationQueueQueryHandler(IAdminGuideVerificationRepository repository) =>
            _repository = repository;

        public Task<IReadOnlyList<AdminGuideVerificationQueueItemResponse>> Handle(
            GetAdminGuideVerificationQueueQuery request,
            CancellationToken cancellationToken) =>
            _repository.GetPendingAsync(cancellationToken);
    }
}
