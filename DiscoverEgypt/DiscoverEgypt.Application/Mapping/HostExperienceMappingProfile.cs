using AutoMapper;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Entities;

namespace DiscoverEgypt.Application.Mapping;

public sealed class HostExperienceMappingProfile : Profile
{
    public HostExperienceMappingProfile()
    {
        CreateMap<Experience, CreateHostExperienceDraftRequest>()
            .ForCtorParam(nameof(CreateHostExperienceDraftRequest.InterestIds), options =>
                options.MapFrom(source => source.ExperienceInterests.Select(link => link.InterestId).ToArray()))
            .ForCtorParam(nameof(CreateHostExperienceDraftRequest.LanguageIds), options =>
                options.MapFrom(source => source.ExperienceLanguages.Select(link => link.LanguageId).ToArray()));

        CreateMap<Experience, HostExperienceDraftEditor>()
            .ForCtorParam(nameof(HostExperienceDraftEditor.ExperienceId), options => options.MapFrom(source => source.Id))
            .ForCtorParam(nameof(HostExperienceDraftEditor.Draft), options => options.MapFrom(source => source));

        CreateMap<Experience, HostExperienceListItem>()
            .ForCtorParam(nameof(HostExperienceListItem.ExperienceType), options => options.MapFrom(source => source.ExperienceType.Name))
            .ForCtorParam(nameof(HostExperienceListItem.CulturalArea), options => options.MapFrom(source => source.CulturalArea.Name))
            .ForCtorParam(nameof(HostExperienceListItem.CoverMediaId), options => options.MapFrom(source =>
                source.Media.Where(media => media.IsCover).Select(media => (int?)media.Id).FirstOrDefault()));

        CreateMap<ExperienceType, HostExperienceLookup>();
        CreateMap<CulturalArea, HostExperienceLookup>();
        CreateMap<Interest, HostExperienceLookup>();
        CreateMap<Language, HostExperienceLookup>();

        CreateMap<ExperienceEvidence, HostExperienceEvidenceFile>()
            .ForCtorParam(nameof(HostExperienceEvidenceFile.Kind), options => options.MapFrom(source => source.EvidenceKind));
    }
}
