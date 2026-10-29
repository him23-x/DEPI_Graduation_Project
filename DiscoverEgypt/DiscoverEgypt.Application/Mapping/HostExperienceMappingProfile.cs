using AutoMapper;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Entities;

using DiscoverEgypt.Domain.Enums.Experiences;

namespace DiscoverEgypt.Application.Mapping;

public sealed class HostExperienceMappingProfile : Profile
{
    public HostExperienceMappingProfile()
    {
        CreateMap<Experience, CreateHostExperienceDraftDto>()
            .ForMember(destination => destination.InterestIds, options =>
                options.MapFrom(source => source.ExperienceInterests.Select(link => link.InterestId).ToArray()))
            .ForMember(destination => destination.LanguageIds, options =>
                options.MapFrom(source => source.ExperienceLanguages.Select(link => link.LanguageId).ToArray()));

        CreateMap<Experience, HostExperienceDraftEditorDto>()
            .ForCtorParam(nameof(HostExperienceDraftEditorDto.ExperienceId), options => options.MapFrom(source => source.Id))
            .ForCtorParam(nameof(HostExperienceDraftEditorDto.Draft), options => options.MapFrom(source => source));

        CreateMap<Experience, HostExperienceListItemDto>()
            .ForCtorParam(nameof(HostExperienceListItemDto.ExperienceType), options => options.MapFrom(source => source.ExperienceType.Name))
            .ForCtorParam(nameof(HostExperienceListItemDto.CulturalArea), options => options.MapFrom(source => source.CulturalArea.Name))
            .ForCtorParam(nameof(HostExperienceListItemDto.CoverMediaId), options => options.MapFrom(source =>
                source.Media.Where(media => media.IsCover).Select(media => (int?)media.Id).FirstOrDefault()));

        CreateMap<ExperienceType, HostExperienceLookupDto>();
        CreateMap<CulturalArea, HostExperienceLookupDto>();
        CreateMap<Interest, HostExperienceLookupDto>();
        CreateMap<Language, HostExperienceLookupDto>();

        CreateMap<ExperienceEvidence, HostExperienceEvidenceFileDto>()
            .ForCtorParam(nameof(HostExperienceEvidenceFileDto.Kind), options => options.MapFrom(source => source.EvidenceKind));
    }
}
