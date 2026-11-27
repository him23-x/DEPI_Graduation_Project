using AutoMapper;
using DiscoverEgypt.Application.Responses.Bookings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostBookingsMappingProfile : Profile
    {
        public HostBookingsMappingProfile()
        {
            CreateMap<Booking, HostBookingListItemResponse>()
                .ForCtorParam(nameof(HostBookingListItemResponse.TouristName), options => options.MapFrom(_ => string.Empty))
                .ForCtorParam(nameof(HostBookingListItemResponse.ExperienceTitle), options => options.MapFrom(source => source.Experience!.Title))
                .ForCtorParam(nameof(HostBookingListItemResponse.NumberOfGuests), options => options.MapFrom(source => source.NumberOfGuests ?? 0))
                .ForMember(destination => destination.PaidAt, options => options.MapFrom(source => source.Payment.PaidAt));

            CreateMap<Booking, HostBookingDetailResponse>()
                .ForCtorParam(nameof(HostBookingDetailResponse.TouristName), options => options.MapFrom(_ => string.Empty))
                .ForCtorParam(nameof(HostBookingDetailResponse.TouristEmail), options => options.MapFrom(_ => string.Empty))
                .ForCtorParam(nameof(HostBookingDetailResponse.ExperienceTitle), options => options.MapFrom(source => source.Experience!.Title))
                .ForCtorParam(nameof(HostBookingDetailResponse.NumberOfGuests), options => options.MapFrom(source => source.NumberOfGuests ?? 0))
                .ForCtorParam(nameof(HostBookingDetailResponse.PaymentStatus), options => options.MapFrom(source => source.Payment.Status))
                .ForCtorParam(nameof(HostBookingDetailResponse.PaidAt), options => options.MapFrom(source => source.Payment.PaidAt))
                .ForCtorParam(nameof(HostBookingDetailResponse.ResponseDeadline), options => options.MapFrom(_ => (DateTime?)null))
                .ForCtorParam(nameof(HostBookingDetailResponse.MeetingPointArea), options => options.MapFrom(source => source.Experience!.MeetingPointArea))
                .ForCtorParam(nameof(HostBookingDetailResponse.ExactMeetingPoint), options => options.MapFrom(source => source.Experience!.ExactMeetingPoint))
                .ForCtorParam(nameof(HostBookingDetailResponse.PayoutAmount), options => options.MapFrom(source => source.Payout == null ? (decimal?)null : source.Payout.Amount))
                .ForCtorParam(nameof(HostBookingDetailResponse.PayoutEligibleAt), options => options.MapFrom(source => source.Payout == null ? (DateTime?)null : source.Payout.EligibleAt))
                .ForCtorParam(nameof(HostBookingDetailResponse.PayoutStatus), options => options.MapFrom(source => source.Payout == null ? (PayoutStatus?)null : source.Payout.Status))
                .ForCtorParam(nameof(HostBookingDetailResponse.HasOpenDispute), options => options.MapFrom(source => source.Disputes.Any(dispute => dispute.Status == DisputeStatus.Open || dispute.Status == DisputeStatus.UnderReview)))
                .ForMember(destination => destination.HasPayout, options => options.MapFrom(source => source.Payout != null));
        }
    }
}
