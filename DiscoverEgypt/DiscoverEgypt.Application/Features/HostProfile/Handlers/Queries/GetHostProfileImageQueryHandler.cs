using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Application.Features.HostProfile.Requests.Queries;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostProfile.Handlers.Queries
{
    public sealed class GetHostProfileImageQueryHandler : IRequestHandler<GetHostProfileImageQuery, ExperienceMediaDownload?>
    {
        private readonly IHostProfileRepository _repository;
        private readonly IExperienceMediaStorage _mediaStorage;

        public GetHostProfileImageQueryHandler(IHostProfileRepository repository, IExperienceMediaStorage mediaStorage)
        {
            _repository = repository;
            _mediaStorage = mediaStorage;
        }

        public async Task<ExperienceMediaDownload?> Handle(GetHostProfileImageQuery request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var profile = await _repository.GetForUserAsync(request.UserId, cancellationToken);
            if (string.IsNullOrWhiteSpace(profile?.ProfileImageKey)) return null;

            try
            {
                return await _mediaStorage.OpenReadAsync(profile.ProfileImageKey, cancellationToken);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}
