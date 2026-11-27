using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.AdminExperiences.Requests.Queries;
using DiscoverEgypt.Application.Responses.AdminExperiences;
using FluentValidation;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminExperiences.Handlers.Queries
{
    public sealed class GetAdminExperienceDetailsQueryHandler
        : IRequestHandler<GetAdminExperienceDetailsQuery, AdminExperienceDetailsResponse?>
    {
        private readonly IAdminExperienceReviewRepository _repository;
        private readonly IValidator<GetAdminExperienceDetailsQuery> _validator;

        public GetAdminExperienceDetailsQueryHandler(
            IAdminExperienceReviewRepository repository,
            IValidator<GetAdminExperienceDetailsQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<AdminExperienceDetailsResponse?> Handle(
            GetAdminExperienceDetailsQuery request,
            CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            return await _repository.GetDetailsAsync(request.ExperienceId, cancellationToken);
        }
    }
}
