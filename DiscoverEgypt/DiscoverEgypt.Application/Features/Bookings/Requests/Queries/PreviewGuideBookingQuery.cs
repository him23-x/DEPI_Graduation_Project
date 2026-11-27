using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.Responses.Bookings;
using MediatR;

namespace DiscoverEgypt.Application.Features.Bookings.Requests.Queries
{
    public sealed record PreviewGuideBookingQuery(GuideBookingInputDto Form) : IRequest<GuideBookingReviewResponse>;
}
