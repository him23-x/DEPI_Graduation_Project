using AutoMapper;
using DiscoverEgypt.Application.DTOs.Disputes;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostDisputesMappingProfile : Profile
    {
        public HostDisputesMappingProfile()
        {
            CreateMap<Dispute, HostDisputeListItemDto>()
                .ForCtorParam(nameof(HostDisputeListItemDto.DisputeId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostDisputeListItemDto.ExperienceTitle), options => options.MapFrom(source => source.Booking.Experience!.Title))
                .ForCtorParam(nameof(HostDisputeListItemDto.GuestName), options => options.MapFrom(_ => string.Empty))
                .ForCtorParam(nameof(HostDisputeListItemDto.ScheduledStart), options => options.MapFrom(source => source.Booking.ScheduledStart))
                .ForCtorParam(nameof(HostDisputeListItemDto.PayoutStatus), options => options.MapFrom(source => source.Booking.Payout == null ? (PayoutStatus?)null : source.Booking.Payout.Status))
                .ForCtorParam(nameof(HostDisputeListItemDto.PayoutAmount), options => options.MapFrom(source => source.Booking.Payout == null ? (decimal?)null : source.Booking.Payout.Amount))
                .ForCtorParam(nameof(HostDisputeListItemDto.EligibleAt), options => options.MapFrom(source => source.Booking.Payout == null ? (DateTime?)null : source.Booking.Payout.EligibleAt))
                .ForCtorParam(nameof(HostDisputeListItemDto.IsPayoutHeld), options => options.MapFrom(_ => false));

            CreateMap<Dispute, HostDisputeDetailsDto>()
                .ForCtorParam(nameof(HostDisputeDetailsDto.DisputeId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostDisputeDetailsDto.ExperienceTitle), options => options.MapFrom(source => source.Booking.Experience!.Title))
                .ForCtorParam(nameof(HostDisputeDetailsDto.GuestName), options => options.MapFrom(_ => string.Empty))
                .ForCtorParam(nameof(HostDisputeDetailsDto.ScheduledStart), options => options.MapFrom(source => source.Booking.ScheduledStart))
                .ForCtorParam(nameof(HostDisputeDetailsDto.ScheduledEnd), options => options.MapFrom(source => source.Booking.ScheduledEnd))
                .ForCtorParam(nameof(HostDisputeDetailsDto.NumberOfGuests), options => options.MapFrom(source => source.Booking.NumberOfGuests ?? 0))
                .ForCtorParam(nameof(HostDisputeDetailsDto.ServiceAmount), options => options.MapFrom(source => source.Booking.ServiceAmount))
                .ForCtorParam(nameof(HostDisputeDetailsDto.BookingStatus), options => options.MapFrom(source => source.Booking.Status))
                .ForCtorParam(nameof(HostDisputeDetailsDto.PayoutAmount), options => options.MapFrom(source => source.Booking.Payout == null ? (decimal?)null : source.Booking.Payout.Amount))
                .ForCtorParam(nameof(HostDisputeDetailsDto.PayoutStatus), options => options.MapFrom(source => source.Booking.Payout == null ? (PayoutStatus?)null : source.Booking.Payout.Status))
                .ForCtorParam(nameof(HostDisputeDetailsDto.EligibleAt), options => options.MapFrom(source => source.Booking.Payout == null ? (DateTime?)null : source.Booking.Payout.EligibleAt))
                .ForCtorParam(nameof(HostDisputeDetailsDto.PaidAt), options => options.MapFrom(source => source.Booking.Payout == null ? (DateTime?)null : source.Booking.Payout.PaidAt))
                .ForCtorParam(nameof(HostDisputeDetailsDto.IsPayoutHeld), options => options.MapFrom(_ => false));
        }
    }
}
