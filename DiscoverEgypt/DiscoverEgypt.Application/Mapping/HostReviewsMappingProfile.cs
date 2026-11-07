using AutoMapper;
using DiscoverEgypt.Application.ViewModels.Reviews;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostReviewsMappingProfile : Profile
    {
        public HostReviewsMappingProfile()
        {
            CreateMap<Review, HostReviewItemViewModel>()
                .ForCtorParam(nameof(HostReviewItemViewModel.ReviewId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostReviewItemViewModel.ExperienceTitle), options => options.MapFrom(source => source.Booking.Experience!.Title))
                .ForCtorParam(nameof(HostReviewItemViewModel.GuestName), options => options.MapFrom(_ => string.Empty));
        }
    }
}
