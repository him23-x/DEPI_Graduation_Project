using DiscoverEgypt.Application.DTOs.TouristPreferences;
using MediatR;

namespace DiscoverEgypt.Application.Features.TouristPreferences.Requests.Queries
{
    public sealed record GetTouristPreferenceQuery(string UserId) : IRequest<TouristPreferenceFormDto>;
}
