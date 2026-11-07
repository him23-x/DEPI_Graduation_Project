using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Commands;
using MediatR;
using AutoMapper;
using FluentValidation;
using DiscoverEgypt.Application.ViewModels.HostExperiences;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Commands
{
    public sealed class UpdateHostExperienceDraftCommandHandler
        : IRequestHandler<UpdateHostExperienceDraftCommand, bool>
    {
        private readonly IHostExperienceRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateHostExperienceDraftCommand> _validator;

        public UpdateHostExperienceDraftCommandHandler(
            IHostExperienceRepository repository,
            IMapper mapper,
            IValidator<UpdateHostExperienceDraftCommand> validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<bool> Handle(UpdateHostExperienceDraftCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            ArgumentNullException.ThrowIfNull(request.Draft);

            var experience = await _repository.GetEditableDraftAsync(
                request.UserId,
                request.ExperienceId,
                cancellationToken);
            if (experience is null)
            {
                return false;
            }

            var types = await _repository.GetExperienceTypesAsync(cancellationToken);
            var areas = await _repository.GetCulturalAreasAsync(cancellationToken);
            var interests = await _repository.GetInterestsAsync(cancellationToken);
            var languages = await _repository.GetLanguagesAsync(cancellationToken);
            var options = new ExperienceCreateOptionsViewModel(
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(types),
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(areas),
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(interests),
                _mapper.Map<IReadOnlyList<ExperienceLookupViewModel>>(languages));
            if (!HostExperienceDraftPolicy.HasValidOptions(request.Draft, options))
            {
                return false;
            }

            var draft = HostExperienceDraftPolicy.Normalize(request.Draft);
            _mapper.Map(draft, experience);
            return await _repository.UpdateDraftAsync(request.UserId, request.ExperienceId, experience, cancellationToken);
        }
    }
}

