using AutoMapper;
using DiscoverEgypt.Application.ViewModels.HostExperiences;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostExperienceMappingProfile : Profile
    {
        public HostExperienceMappingProfile()
        {
            CreateMap<HostExperienceDraftViewModel, Experience>()
                .ForMember(destination => destination.ExperienceInterests, options => options.MapFrom(source =>
                    source.InterestIds.Select(id => new ExperienceInterest { InterestId = id })))
                .ForMember(destination => destination.ExperienceLanguages, options => options.MapFrom(source =>
                    source.LanguageIds.Select(id => new ExperienceLanguage { LanguageId = id })));

            CreateMap<Experience, HostExperienceDraftViewModel>()
                .ForMember(destination => destination.InterestIds, options =>
                    options.MapFrom(source => source.ExperienceInterests.Select(link => link.InterestId).ToArray()))
                .ForMember(destination => destination.LanguageIds, options =>
                    options.MapFrom(source => source.ExperienceLanguages.Select(link => link.LanguageId).ToArray()));

            CreateMap<Experience, HostExperienceDraftEditorViewModel>()
                .ForCtorParam(nameof(HostExperienceDraftEditorViewModel.ExperienceId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostExperienceDraftEditorViewModel.Draft), options => options.MapFrom(source => source));

            CreateMap<Experience, ExperienceListItemViewModel>()
                .ForCtorParam(nameof(ExperienceListItemViewModel.ExperienceType), options => options.MapFrom(source => source.ExperienceType.Name))
                .ForCtorParam(nameof(ExperienceListItemViewModel.CulturalArea), options => options.MapFrom(source => source.CulturalArea.Name))
                .ForCtorParam(nameof(ExperienceListItemViewModel.CoverMediaId), options => options.MapFrom(source =>
                    source.Media.Where(media => media.IsCover).Select(media => (int?)media.Id).FirstOrDefault()));

            CreateMap<Experience, ExperienceMediaViewModel>()
                .ForCtorParam(nameof(ExperienceMediaViewModel.ExperienceId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(ExperienceMediaViewModel.Media), options => options.MapFrom(source => source.Media));
            CreateMap<ExperienceMedia, ExperienceMediaItemViewModel>()
                .ForCtorParam(nameof(ExperienceMediaItemViewModel.ContentType), options => options.MapFrom(_ => string.Empty));

            CreateMap<Experience, ExperienceEvidenceEditorViewModel>()
                .ForCtorParam(nameof(ExperienceEvidenceEditorViewModel.ExperienceId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(ExperienceEvidenceEditorViewModel.ExperienceTypeName), options => options.MapFrom(source => source.ExperienceType.Name))
                .ForCtorParam(nameof(ExperienceEvidenceEditorViewModel.ExperienceType), options => options.MapFrom(source => HostExperienceSubmissionPolicy.Classify(source.ExperienceType.Name)))
                .ForCtorParam(nameof(ExperienceEvidenceEditorViewModel.Files), options => options.MapFrom(_ => Array.Empty<ExperienceEvidenceFileViewModel>()));

            CreateMap<Experience, ExperienceSubmissionReviewViewModel>()
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.ExperienceId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.ExperienceTypeName), options => options.MapFrom(source => source.ExperienceType.Name))
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.ExperienceType), options => options.MapFrom(source => HostExperienceSubmissionPolicy.Classify(source.ExperienceType.Name)))
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.CulturalArea), options => options.MapFrom(source => source.CulturalArea.Name))
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.ImageCount), options => options.MapFrom(source => source.Media.Count(media => media.MediaType == MediaType.Image)))
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.VideoCount), options => options.MapFrom(source => source.Media.Count(media => media.MediaType == MediaType.Video)))
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.HasCoverImage), options => options.MapFrom(source => source.Media.Any(media => media.MediaType == MediaType.Image && media.IsCover)))
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.Evidence), options => options.MapFrom(source => source.Evidence
                    .GroupBy(item => item.EvidenceKind)
                    .Select(group => new ExperienceEvidenceSummaryViewModel(group.Key, group.FirstOrDefault(item => item.TextContent != null) == null ? null : group.FirstOrDefault(item => item.TextContent != null)!.TextContent, group.Count(item => item.StorageKey != null)))))
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.HasInterest), options => options.MapFrom(source => source.ExperienceInterests.Any()))
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.HasLanguage), options => options.MapFrom(source => source.ExperienceLanguages.Any()))
                .ForCtorParam(nameof(ExperienceSubmissionReviewViewModel.Issues), options => options.MapFrom(_ => Array.Empty<string>()));

            CreateMap<ExperienceType, ExperienceLookupViewModel>();
            CreateMap<CulturalArea, ExperienceLookupViewModel>();
            CreateMap<Interest, ExperienceLookupViewModel>();
            CreateMap<Language, ExperienceLookupViewModel>();

            CreateMap<ExperienceEvidence, ExperienceEvidenceFileViewModel>()
                .ForCtorParam(nameof(ExperienceEvidenceFileViewModel.Kind), options => options.MapFrom(source => source.EvidenceKind));
        }
    }
}

