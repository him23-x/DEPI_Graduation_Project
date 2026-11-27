using AutoMapper;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Application.Features.HostExperiences.Policies;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Mapping
{
    public sealed class HostExperienceMappingProfile : Profile
    {
        public HostExperienceMappingProfile()
        {
            CreateMap<HostExperienceDraftDto, Experience>()
                .ForMember(destination => destination.ExperienceInterests, options => options.MapFrom(source =>
                    source.InterestIds.Select(id => new ExperienceInterest { InterestId = id })))
                .ForMember(destination => destination.ExperienceLanguages, options => options.MapFrom(source =>
                    source.LanguageIds.Select(id => new ExperienceLanguage { LanguageId = id })));

            CreateMap<Experience, HostExperienceDraftDto>()
                .ForMember(destination => destination.InterestIds, options =>
                    options.MapFrom(source => source.ExperienceInterests.Select(link => link.InterestId).ToArray()))
                .ForMember(destination => destination.LanguageIds, options =>
                    options.MapFrom(source => source.ExperienceLanguages.Select(link => link.LanguageId).ToArray()));

            CreateMap<Experience, HostExperienceDraftEditorDto>()
                .ForCtorParam(nameof(HostExperienceDraftEditorDto.ExperienceId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(HostExperienceDraftEditorDto.Draft), options => options.MapFrom(source => source));

            CreateMap<Experience, ExperienceListItemDto>()
                .ForCtorParam(nameof(ExperienceListItemDto.ExperienceType), options => options.MapFrom(source => source.ExperienceType.Name))
                .ForCtorParam(nameof(ExperienceListItemDto.CulturalArea), options => options.MapFrom(source => source.CulturalArea.Name))
                .ForCtorParam(nameof(ExperienceListItemDto.CoverMediaId), options => options.MapFrom(source =>
                    source.Media.Where(media => media.IsCover).Select(media => (int?)media.Id).FirstOrDefault()));

            CreateMap<Experience, ExperienceMediaDto>()
                .ForCtorParam(nameof(ExperienceMediaDto.ExperienceId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(ExperienceMediaDto.Media), options => options.MapFrom(source => source.Media));
            CreateMap<ExperienceMedia, ExperienceMediaItemDto>()
                .ForCtorParam(nameof(ExperienceMediaItemDto.ContentType), options => options.MapFrom(_ => string.Empty));

            CreateMap<Experience, ExperienceEvidenceEditorDto>()
                .ConvertUsing(source => new ExperienceEvidenceEditorDto(
                    source.Id,
                    source.Title,
                    source.ExperienceType.Name,
                    HostExperienceSubmissionPolicy.Classify(source.ExperienceType.Name),
                    null,
                    Array.Empty<ExperienceEvidenceFileDto>()));

            CreateMap<Experience, ExperienceSubmissionReviewDto>()
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.ExperienceId), options => options.MapFrom(source => source.Id))
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.ExperienceTypeName), options => options.MapFrom(source => source.ExperienceType.Name))
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.ExperienceType), options => options.MapFrom(source => HostExperienceSubmissionPolicy.Classify(source.ExperienceType.Name)))
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.CulturalArea), options => options.MapFrom(source => source.CulturalArea.Name))
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.ImageCount), options => options.MapFrom(source => source.Media.Count(media => media.MediaType == MediaType.Image)))
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.VideoCount), options => options.MapFrom(source => source.Media.Count(media => media.MediaType == MediaType.Video)))
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.HasCoverImage), options => options.MapFrom(source => source.Media.Any(media => media.MediaType == MediaType.Image && media.IsCover)))
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.Evidence), options => options.MapFrom(source => source.Evidence
                    .GroupBy(item => item.EvidenceKind)
                    .Select(group => new ExperienceEvidenceSummaryDto(group.Key, group.FirstOrDefault(item => item.TextContent != null) == null ? null : group.FirstOrDefault(item => item.TextContent != null)!.TextContent, group.Count(item => item.StorageKey != null)))))
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.HasInterest), options => options.MapFrom(source => source.ExperienceInterests.Any()))
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.HasLanguage), options => options.MapFrom(source => source.ExperienceLanguages.Any()))
                .ForCtorParam(nameof(ExperienceSubmissionReviewDto.Issues), options => options.MapFrom(_ => Array.Empty<string>()));

            CreateMap<ExperienceType, ExperienceLookupDto>();
            CreateMap<CulturalArea, ExperienceLookupDto>();
            CreateMap<Interest, ExperienceLookupDto>();
            CreateMap<Language, ExperienceLookupDto>();

            CreateMap<ExperienceEvidence, ExperienceEvidenceFileDto>()
                .ForCtorParam(nameof(ExperienceEvidenceFileDto.Kind), options => options.MapFrom(source => source.EvidenceKind));
        }
    }
}

