using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.ApprovedGuides.Requests.Queries;
using DiscoverEgypt.Application.Responses.Guides;
using MediatR;

namespace DiscoverEgypt.Application.Features.ApprovedGuides.Handlers.Queries
{
    public sealed class GetApprovedGuidesQueryHandler : IRequestHandler<GetApprovedGuidesQuery, ApprovedGuidesResponse>
    {
        private const int MaximumPageSize = 24;
        private readonly IApprovedGuideRepository _repository;

        public GetApprovedGuidesQueryHandler(IApprovedGuideRepository repository) => _repository = repository;

        public async Task<ApprovedGuidesResponse> Handle(GetApprovedGuidesQuery request, CancellationToken cancellationToken)
        {
            var search = request.Search?.Trim() ?? string.Empty;
            var languageId = request.LanguageId is > 0 ? request.LanguageId : null;
            var culturalAreaId = request.CulturalAreaId is > 0 ? request.CulturalAreaId : null;
            var pageSize = Math.Clamp(request.PageSize, 1, MaximumPageSize);
            var totalCount = await _repository.CountAsync(search, languageId, culturalAreaId, cancellationToken);
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            var page = Math.Clamp(request.Page, 1, totalPages);
            var items = await _repository.GetPageAsync(search, languageId, culturalAreaId,
                (page - 1) * pageSize, pageSize, cancellationToken);
            var languages = await _repository.GetLanguagesAsync(cancellationToken);
            var areas = await _repository.GetCulturalAreasAsync(cancellationToken);

            return new ApprovedGuidesResponse(items, languages, areas, search, languageId, culturalAreaId, page, pageSize, totalCount);
        }
    }
}
