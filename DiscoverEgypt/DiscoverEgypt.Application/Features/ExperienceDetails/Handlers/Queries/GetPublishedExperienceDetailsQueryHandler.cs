using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.ExperienceDetails;
using DiscoverEgypt.Application.Features.ExperienceDetails.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.ExperienceDetails.Handlers.Queries
{
    public sealed class GetPublishedExperienceDetailsQueryHandler : IRequestHandler<GetPublishedExperienceDetailsQuery, ExperienceDetailsDto?>
    {
        private readonly IExperienceDetailsRepository _repository;

        public GetPublishedExperienceDetailsQueryHandler(IExperienceDetailsRepository repository) => _repository = repository;

        public Task<ExperienceDetailsDto?> Handle(GetPublishedExperienceDetailsQuery request, CancellationToken cancellationToken)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.ExperienceId);
            return _repository.GetPublishedDetailsAsync(request.ExperienceId, cancellationToken);
        }
    }
}
