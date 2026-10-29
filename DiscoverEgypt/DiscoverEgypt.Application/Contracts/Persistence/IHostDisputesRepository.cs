using DiscoverEgypt.Application.DTOs.Disputes;

namespace DiscoverEgypt.Application.Contracts.Persistence;

public interface IHostDisputesRepository
{
    Task<HostDisputesPageDto> GetForHostAsync(
        string userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<HostDisputeDetailsDto?> GetDetailsAsync(
        string userId,
        int disputeId,
        CancellationToken cancellationToken = default);
}
