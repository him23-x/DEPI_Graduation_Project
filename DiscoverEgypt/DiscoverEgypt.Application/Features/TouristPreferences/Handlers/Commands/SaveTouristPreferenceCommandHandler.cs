using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.TouristPreferences.Requests.Commands;
using MediatR;

namespace DiscoverEgypt.Application.Features.TouristPreferences.Handlers.Commands
{
    public sealed class SaveTouristPreferenceCommandHandler : IRequestHandler<SaveTouristPreferenceCommand, Unit>
    {
        private readonly ITouristPreferenceRepository _repository;

        public SaveTouristPreferenceCommandHandler(ITouristPreferenceRepository repository) => _repository = repository;

        public async Task<Unit> Handle(SaveTouristPreferenceCommand request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            ArgumentNullException.ThrowIfNull(request.Form);

            await _repository.SaveAsync(
                request.UserId,
                request.Form.Budget!.Value,
                request.Form.Duration!.Value,
                request.Form.GuidePreference!.Value,
                request.Form.InterestIds,
                request.Form.ExperienceTypeIds,
                request.Form.CulturalAreaIds,
                request.Form.LanguageIds,
                cancellationToken);

            return Unit.Value;
        }
    }
}
