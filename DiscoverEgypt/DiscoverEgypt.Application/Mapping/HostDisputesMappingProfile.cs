using AutoMapper;
using DiscoverEgypt.Application.ViewModels.Disputes;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostDisputesMappingProfile : Profile
    {
        public HostDisputesMappingProfile()
        {
            CreateMap<Dispute, HostDisputeListItemViewModel>()
                .ForCtorParam(nameof(HostDisputeListItemViewModel.DisputeId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostDisputeListItemViewModel.ExperienceTitle), options => options.MapFrom(source => source.Booking.Experience!.Title))
                .ForCtorParam(nameof(HostDisputeListItemViewModel.GuestName), options => options.MapFrom(_ => string.Empty))
                .ForCtorParam(nameof(HostDisputeListItemViewModel.ScheduledStart), options => options.MapFrom(source => source.Booking.ScheduledStart))
                .ForCtorParam(nameof(HostDisputeListItemViewModel.PayoutStatus), options => options.MapFrom(source => source.Booking.Payout == null ? (PayoutStatus?)null : source.Booking.Payout.Status))
                .ForCtorParam(nameof(HostDisputeListItemViewModel.PayoutAmount), options => options.MapFrom(source => source.Booking.Payout == null ? (decimal?)null : source.Booking.Payout.Amount))
                .ForCtorParam(nameof(HostDisputeListItemViewModel.EligibleAt), options => options.MapFrom(source => source.Booking.Payout == null ? (DateTime?)null : source.Booking.Payout.EligibleAt))
                .ForCtorParam(nameof(HostDisputeListItemViewModel.IsPayoutHeld), options => options.MapFrom(_ => false));

            CreateMap<Dispute, HostDisputeDetailsViewModel>()
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.DisputeId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.ExperienceTitle), options => options.MapFrom(source => source.Booking.Experience!.Title))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.GuestName), options => options.MapFrom(_ => string.Empty))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.ScheduledStart), options => options.MapFrom(source => source.Booking.ScheduledStart))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.ScheduledEnd), options => options.MapFrom(source => source.Booking.ScheduledEnd))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.NumberOfGuests), options => options.MapFrom(source => source.Booking.NumberOfGuests ?? 0))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.ServiceAmount), options => options.MapFrom(source => source.Booking.ServiceAmount))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.BookingStatus), options => options.MapFrom(source => source.Booking.Status))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.PayoutAmount), options => options.MapFrom(source => source.Booking.Payout == null ? (decimal?)null : source.Booking.Payout.Amount))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.PayoutStatus), options => options.MapFrom(source => source.Booking.Payout == null ? (PayoutStatus?)null : source.Booking.Payout.Status))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.EligibleAt), options => options.MapFrom(source => source.Booking.Payout == null ? (DateTime?)null : source.Booking.Payout.EligibleAt))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.PaidAt), options => options.MapFrom(source => source.Booking.Payout == null ? (DateTime?)null : source.Booking.Payout.PaidAt))
                .ForCtorParam(nameof(HostDisputeDetailsViewModel.IsPayoutHeld), options => options.MapFrom(_ => false));
        }
    }
}
