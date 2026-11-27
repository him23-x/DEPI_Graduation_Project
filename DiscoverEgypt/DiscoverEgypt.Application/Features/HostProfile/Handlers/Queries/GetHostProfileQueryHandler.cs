using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.HostProfile.Requests.Queries;
using DiscoverEgypt.Application.DTOs.HostProfile;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostProfile.Handlers.Queries
{
    public sealed class GetHostProfileQueryHandler : IRequestHandler<GetHostProfileQuery, HostProfileDto?>
    {
        private readonly IHostProfileRepository _repository;

        public GetHostProfileQueryHandler(IHostProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<HostProfileDto?> Handle(GetHostProfileQuery request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);

            var profile = await _repository.GetForUserAsync(request.UserId, cancellationToken);
            return profile is null
                ? null
                : new HostProfileDto
                {
                    HostType = profile.HostType,
                    Bio = profile.Bio,
                    HasProfileImage = !string.IsNullOrWhiteSpace(profile.ProfileImageKey)
                };
        }
    }
}
