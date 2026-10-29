using AutoMapper;
using DiscoverEgypt.Application.DTOs.Bookings;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.DTOs.Notifications;
using DiscoverEgypt.Presentation.ViewModels.Experience;
using DiscoverEgypt.Presentation.ViewModels.Bookings;
using DiscoverEgypt.Presentation.ViewModels.Notifications;

namespace DiscoverEgypt.Presentation.Mapping;

public sealed class PresentationMappingProfile : Profile
{
    public PresentationMappingProfile()
    {
        CreateMap<HostBookingListItemDto, HostBookingListItemViewModel>();
        CreateMap<HostBookingsIndexDto, HostBookingsIndexViewModel>();
        CreateMap<HostBookingDetailDto, BookingDetailsViewModel>();
        CreateMap<HostBookingDetailsDto, HostBookingDetailsViewModel>();
        CreateMap<HostNotificationItemDto, HostNotificationViewModel>();
        CreateMap<HostNotificationsPageDto, HostNotificationsViewModel>();

        CreateMap<HostExperienceLookupDto, ExperienceLookupViewModel>();
        CreateMap<HostExperienceCreateOptionsDto, ExperienceCreateOptionsViewModel>();
        CreateMap<CreateExperienceDto, CreateExperienceViewModel>();
        CreateMap<CreateExperienceViewModel, CreateExperienceDto>();
        CreateMap<CreateExperienceViewModel, CreateHostExperienceDraftDto>();
        CreateMap<HostExperienceDraftEditorDto, CreateExperienceViewModel>()
            .ForMember(destination => destination.ExperienceTypeId, options => options.MapFrom(source => source.Draft.ExperienceTypeId))
            .ForMember(destination => destination.CulturalAreaId, options => options.MapFrom(source => source.Draft.CulturalAreaId))
            .ForMember(destination => destination.Title, options => options.MapFrom(source => source.Draft.Title))
            .ForMember(destination => destination.Description, options => options.MapFrom(source => source.Draft.Description))
            .ForMember(destination => destination.CulturalStory, options => options.MapFrom(source => source.Draft.CulturalStory))
            .ForMember(destination => destination.PricePerPerson, options => options.MapFrom(source => source.Draft.PricePerPerson))
            .ForMember(destination => destination.DurationMinutes, options => options.MapFrom(source => source.Draft.DurationMinutes))
            .ForMember(destination => destination.MaxGroupSize, options => options.MapFrom(source => source.Draft.MaxGroupSize))
            .ForMember(destination => destination.MeetingPointArea, options => options.MapFrom(source => source.Draft.MeetingPointArea))
            .ForMember(destination => destination.ExactMeetingPoint, options => options.MapFrom(source => source.Draft.ExactMeetingPoint))
            .ForMember(destination => destination.InterestIds, options => options.MapFrom(source => source.Draft.InterestIds))
            .ForMember(destination => destination.LanguageIds, options => options.MapFrom(source => source.Draft.LanguageIds));
        CreateMap<HostExperienceListItemDto, ExperienceListItemViewModel>();
        CreateMap<ExperiencesIndexDto, ExperiencesIndexViewModel>();
        CreateMap<HostExperienceMediaItemDto, ExperienceMediaItemViewModel>();
        CreateMap<HostExperienceMediaEditorDto, ExperienceMediaViewModel>();
        CreateMap<HostExperienceEvidenceFileDto, ExperienceEvidenceFileViewModel>();
        CreateMap<HostExperienceEvidenceEditorDto, ExperienceEvidenceEditorViewModel>();
        CreateMap<EvidenceFileGroupDto, EvidenceFileGroupViewModel>();
        CreateMap<ExperienceEvidenceDto, ExperienceEvidenceViewModel>();
        CreateMap<HostExperienceEvidenceSummaryDto, ExperienceEvidenceSummaryViewModel>();
        CreateMap<HostExperienceSubmissionReviewDto, ExperienceSubmissionReviewViewModel>();
        CreateMap<ExperienceReviewDto, ExperienceReviewViewModel>();
        CreateMap<SaveExperienceEvidenceTextViewModel, SaveExperienceEvidenceTextDto>();
        CreateMap<SubmitExperienceViewModel, SubmitExperienceDto>();
    }
}
