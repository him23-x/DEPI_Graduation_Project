using AutoMapper;
using DiscoverEgypt.Application.ViewModels.Dashboard;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostDashboardMappingProfile : Profile
    {
        public HostDashboardMappingProfile()
        {
            CreateMap<Booking, HostDashboardBookingViewModel>()
                .ForCtorParam(nameof(HostDashboardBookingViewModel.BookingId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostDashboardBookingViewModel.ExperienceTitle), options => options.MapFrom(source => source.Experience!.Title));
        }
    }
}
