using DiscoverEgypt.Application.DTOs.TouristPreferences;
using MediatR;

namespace DiscoverEgypt.Application.Features.TouristPreferences.Requests.Commands
{
    public sealed record SaveTouristPreferenceCommand(string UserId, TouristPreferenceFormDto Form) : IRequest<Unit>;
}
