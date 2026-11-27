using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Explore.Requests.Queries;
using DiscoverEgypt.Application.Responses.Experiences;
using MediatR;

namespace DiscoverEgypt.Application.Features.Explore.Handlers.Queries
{
    public sealed class ExploreExperiencesQueryHandler : IRequestHandler<ExploreExperiencesQuery, ExploreExperiencesResponse>
    {
        private const int MaximumPageSize = 24;
        private static readonly string[] AllowedSorts = ["newest", "price-low", "price-high", "rating"];
        private readonly IExperienceExploreRepository _repository;

        public ExploreExperiencesQueryHandler(IExperienceExploreRepository repository)
        {
            _repository = repository;
        }

        public async Task<ExploreExperiencesResponse> Handle(ExploreExperiencesQuery request, CancellationToken cancellationToken)
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, MaximumPageSize);
            var search = request.Search?.Trim() ?? string.Empty;
            var sort = AllowedSorts.Contains(request.Sort, StringComparer.OrdinalIgnoreCase)
                ? request.Sort!.ToLowerInvariant()
                : "newest";
            var minPrice = request.MinPrice is >= 0 ? request.MinPrice : null;
            var maxPrice = request.MaxPrice is >= 0 ? request.MaxPrice : null;
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                (minPrice, maxPrice) = (maxPrice, minPrice);

            var totalCount = await _repository.CountAsync(
                search, request.TypeId, request.AreaId, minPrice, maxPrice, cancellationToken);
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            page = Math.Min(page, totalPages);

            var items = await _repository.GetPageAsync(
                search,
                request.TypeId,
                request.AreaId,
                minPrice,
                maxPrice,
                sort,
                (page - 1) * pageSize,
                pageSize,
                cancellationToken);
            var types = await _repository.GetTypesAsync(cancellationToken);
            var areas = await _repository.GetAreasAsync(cancellationToken);

            return new ExploreExperiencesResponse(
                items, types, areas, page, pageSize, totalCount,
                search, request.TypeId, request.AreaId, minPrice, maxPrice, sort);
        }
    }
}
