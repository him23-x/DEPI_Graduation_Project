using AutoMapper;
using DiscoverEgypt.Application.ViewModels.Bookings;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Disputes;
using DiscoverEgypt.Domain.Enums.Payments;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostBookingsMappingProfile : Profile
    {
        public HostBookingsMappingProfile()
        {
            CreateMap<Booking, HostBookingListItemViewModel>()
                .ForCtorParam(nameof(HostBookingListItemViewModel.TouristName), options => options.MapFrom(_ => string.Empty))
                .ForCtorParam(nameof(HostBookingListItemViewModel.ExperienceTitle), options => options.MapFrom(source => source.Experience!.Title))
                .ForCtorParam(nameof(HostBookingListItemViewModel.NumberOfGuests), options => options.MapFrom(source => source.NumberOfGuests ?? 0))
                .ForMember(destination => destination.PaidAt, options => options.MapFrom(source => source.Payment.PaidAt));

            CreateMap<Booking, BookingDetailsViewModel>()
                .ForCtorParam(nameof(BookingDetailsViewModel.TouristName), options => options.MapFrom(_ => string.Empty))
                .ForCtorParam(nameof(BookingDetailsViewModel.TouristEmail), options => options.MapFrom(_ => string.Empty))
                .ForCtorParam(nameof(BookingDetailsViewModel.ExperienceTitle), options => options.MapFrom(source => source.Experience!.Title))
                .ForCtorParam(nameof(BookingDetailsViewModel.NumberOfGuests), options => options.MapFrom(source => source.NumberOfGuests ?? 0))
                .ForCtorParam(nameof(BookingDetailsViewModel.PaymentStatus), options => options.MapFrom(source => source.Payment.Status))
                .ForCtorParam(nameof(BookingDetailsViewModel.PaidAt), options => options.MapFrom(source => source.Payment.PaidAt))
                .ForCtorParam(nameof(BookingDetailsViewModel.ResponseDeadline), options => options.MapFrom(_ => (DateTime?)null))
                .ForCtorParam(nameof(BookingDetailsViewModel.MeetingPointArea), options => options.MapFrom(source => source.Experience!.MeetingPointArea))
                .ForCtorParam(nameof(BookingDetailsViewModel.ExactMeetingPoint), options => options.MapFrom(source => source.Experience!.ExactMeetingPoint))
                .ForCtorParam(nameof(BookingDetailsViewModel.PayoutAmount), options => options.MapFrom(source => source.Payout == null ? (decimal?)null : source.Payout.Amount))
                .ForCtorParam(nameof(BookingDetailsViewModel.PayoutEligibleAt), options => options.MapFrom(source => source.Payout == null ? (DateTime?)null : source.Payout.EligibleAt))
                .ForCtorParam(nameof(BookingDetailsViewModel.PayoutStatus), options => options.MapFrom(source => source.Payout == null ? (PayoutStatus?)null : source.Payout.Status))
                .ForCtorParam(nameof(BookingDetailsViewModel.HasOpenDispute), options => options.MapFrom(source => source.Disputes.Any(dispute => dispute.Status == DisputeStatus.Open || dispute.Status == DisputeStatus.UnderReview)))
                .ForMember(destination => destination.HasPayout, options => options.MapFrom(source => source.Payout != null));
        }
    }
}
