using AutoMapper;
using DiscoverEgypt.Application.Responses.Dashboard;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostDashboardMappingProfile : Profile
    {
        public HostDashboardMappingProfile()
        {
            CreateMap<Booking, HostDashboardBookingResponse>()
                .ForCtorParam(nameof(HostDashboardBookingResponse.BookingId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostDashboardBookingResponse.ExperienceTitle), options => options.MapFrom(source => source.Experience!.Title));
        }
    }
}
