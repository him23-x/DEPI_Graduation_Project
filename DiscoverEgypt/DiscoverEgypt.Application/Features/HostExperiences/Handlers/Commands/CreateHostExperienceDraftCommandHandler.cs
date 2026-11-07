using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;
using AutoMapper;
using FluentValidation;
using DiscoverEgypt.Application.ViewModels.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class CreateHostExperienceDraftCommandHandler
        : IRequestHandler<CreateHostExperienceDraftCommand, int?>
    {
        private readonly IHostExperienceRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateHostExperienceDraftCommand> _validator;

        public CreateHostExperienceDraftCommandHandler(
            IHostExperienceRepository repository,
            IMapper mapper,
            IValidator<CreateHostExperienceDraftCommand> validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<int?> Handle(CreateHostExperienceDraftCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            ArgumentNullException.ThrowIfNull(request.Draft);

            var options = await GetCreateOptionsAsync(cancellationToken);
            if (!HostExperienceDraftPolicy.HasValidOptions(request.Draft, options))
            {
                return null;
            }

            var draft = HostExperienceDraftPolicy.Normalize(request.Draft);
            var experience = _mapper.Map<DiscoverEgypt.Domain.Entities.Experience>(draft);
            return await _repository.CreateDraftAsync(request.UserId, experience, cancellationToken);
        }

        private async Task<ExperienceCreateOptionsViewModel> GetCreateOptionsAsync(CancellationToken cancellationToken)
        {
            var types = await _repository.GetExperienceTypesAsync(cancellationToken);
            var areas = await _repository.GetCulturalAreasAsync(cancellationToken);
            var interests = await _repository.GetInterestsAsync(cancellationToken);
            var languages = await _repository.GetLanguagesAsync(cancellationToken);
            return new ExperienceCreateOptionsViewModel(
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(types),
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(areas),
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(interests),
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(languages));
        }
    }
}

