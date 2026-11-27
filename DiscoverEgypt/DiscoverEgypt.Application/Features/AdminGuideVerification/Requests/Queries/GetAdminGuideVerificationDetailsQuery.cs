using DiscoverEgypt.Application.Responses.GuideVerification;
using MediatR;

namespace DiscoverEgypt.Application.Features.AdminGuideVerification.Requests.Queries
{
    public sealed record GetAdminGuideVerificationDetailsQuery(int VerificationRequestId)
        : IRequest<AdminGuideVerificationDetailsResponse?>;
}
