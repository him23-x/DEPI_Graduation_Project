using AutoMapper;
using DiscoverEgypt.Application.Responses.Reviews;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostReviewsMappingProfile : Profile
    {
        public HostReviewsMappingProfile()
        {
            CreateMap<Review, HostReviewItemResponse>()
                .ForCtorParam(nameof(HostReviewItemResponse.ReviewId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostReviewItemResponse.ExperienceTitle), options => options.MapFrom(source => source.Booking.Experience!.Title))
                .ForCtorParam(nameof(HostReviewItemResponse.GuestName), options => options.MapFrom(_ => string.Empty));
        }
    }
}
