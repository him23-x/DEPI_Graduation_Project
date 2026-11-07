using AutoMapper;
using DiscoverEgypt.Application.ViewModels.Earnings;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostEarningsMappingProfile : Profile
    {
        public HostEarningsMappingProfile()
        {
            CreateMap<Payout, HostPayoutItemViewModel>()
                .ForCtorParam(nameof(HostPayoutItemViewModel.PayoutId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostPayoutItemViewModel.ExperienceTitle), options => options.MapFrom(source => source.Booking.Experience!.Title))
                .ForCtorParam(nameof(HostPayoutItemViewModel.ScheduledStart), options => options.MapFrom(source => source.Booking.ScheduledStart));
        }
    }
}
