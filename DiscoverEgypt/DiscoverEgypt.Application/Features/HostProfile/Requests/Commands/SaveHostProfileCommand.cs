using DiscoverEgypt.Application.DTOs.HostProfile;
using MediatR;

namespace DiscoverEgypt.Application.Features.HostProfile.Requests.Commands
{
    public sealed record SaveHostProfileCommand(
        string UserId,
        HostProfileDto Profile,
        Stream? ImageContent = null,
        string? ImageContentType = null,
        string? ImageExtension = null,
        long ImageLength = 0) : IRequest<Unit>;
}
