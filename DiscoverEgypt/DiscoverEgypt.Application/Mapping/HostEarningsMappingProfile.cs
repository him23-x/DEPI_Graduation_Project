using AutoMapper;
using DiscoverEgypt.Application.Responses.Earnings;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostEarningsMappingProfile : Profile
    {
        public HostEarningsMappingProfile()
        {
            CreateMap<Payout, HostPayoutItemResponse>()
                .ForCtorParam(nameof(HostPayoutItemResponse.PayoutId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostPayoutItemResponse.ExperienceTitle), options => options.MapFrom(source => source.Booking.Experience!.Title))
                .ForCtorParam(nameof(HostPayoutItemResponse.ScheduledStart), options => options.MapFrom(source => source.Booking.ScheduledStart));
        }
    }
}
