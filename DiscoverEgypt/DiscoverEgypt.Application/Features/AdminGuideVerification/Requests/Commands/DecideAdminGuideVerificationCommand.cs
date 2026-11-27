using DiscoverEgypt.Application.DTOs.GuideVerification;
using DiscoverEgypt.Application.Enums.GuideVerification;
using DiscoverEgypt.Application.Responses.GuideVerification;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Commands
{
    public sealed record DecideAdminGuideVerificationCommand(
        string AdminUserId,
        int VerificationRequestId,
        GuideVerificationDecision Decision,
        AdminGuideVerificationDecisionDto Form)
        : IRequest<AdminGuideVerificationDecisionResponse>;
}
