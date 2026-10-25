using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscoverEgypt.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IExperienceMediaStorage>(_ => new AzureExperienceMediaStorage(
            configuration["ExperienceMediaStorage:ServiceUri"],
            configuration["ExperienceMediaStorage:ConnectionString"],
            configuration["ExperienceMediaStorage:ContainerName"]));

        services.AddSingleton<IExperienceEvidenceStorage>(_ => new AzureExperienceEvidenceStorage(
            configuration["ExperienceEvidenceStorage:ServiceUri"],
            configuration["ExperienceEvidenceStorage:ConnectionString"],
            configuration["ExperienceEvidenceStorage:ContainerName"]));

        return services;
    }
}
