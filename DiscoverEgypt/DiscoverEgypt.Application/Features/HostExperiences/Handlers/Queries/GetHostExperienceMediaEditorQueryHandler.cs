using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Requests.Queries;
using MediatR;
using AutoMapper;

namespace DiscoverEgypt.Application.Features.HostExperiences.Handlers.Queries
{
    public sealed class GetHostExperienceMediaEditorQueryHandler
        : IRequestHandler<GetHostExperienceMediaEditorQuery, ExperienceMediaDto?>
    {
        private readonly IHostExperienceMediaRepository _repository;
        private readonly IMapper _mapper;

        public GetHostExperienceMediaEditorQueryHandler(IHostExperienceMediaRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ExperienceMediaDto?> Handle(GetHostExperienceMediaEditorQuery request, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.UserId);
            var experience = await _repository.GetDraftEditorAsync(request.UserId, request.ExperienceId, cancellationToken);
            if (experience is null) return null;

            var model = _mapper.Map<ExperienceMediaDto>(experience);
            return model with { Media = model.Media.Select(item => item with
            {
                ContentType = GetContentType(experience.Media.Single(media => media.Id == item.Id).StorageKey, item.MediaType)
            }).ToArray() };
        }

        private static string GetContentType(string storageKey, DiscoverEgypt.Domain.Enums.Experiences.MediaType mediaType)
        {
            var extension = Path.GetExtension(storageKey).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".mp4" => "video/mp4",
                _ => mediaType == DiscoverEgypt.Domain.Enums.Experiences.MediaType.Image ? "image/jpeg" : "video/mp4"
            };
        }
    }
}

