using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Application.Responses.Disputes;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminDisputes.Requests.Commands
{
    public sealed record ResolveAdminDisputeCommand(
        string AdminUserId,
        int DisputeId,
        AdminDisputeResolutionDto Form)
        : IRequest<AdminDisputeResolutionResponse>;
}
