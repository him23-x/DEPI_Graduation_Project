using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscoverEgypt.Infrastructure;

public static class InfrastructureServicesRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var provider = configuration["ExperienceStorage:Provider"];
        if (string.IsNullOrWhiteSpace(provider))
            provider = environment.IsDevelopment() ? "Local" : "Azure";

        if (provider.Equals("Local", StringComparison.OrdinalIgnoreCase))
        {
            var rootPath = configuration["ExperienceStorage:LocalRootPath"];
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                rootPath = string.IsNullOrWhiteSpace(localAppData)
                    ? Path.Combine(AppContext.BaseDirectory, "App_Data", "ExperienceStorage")
                    : Path.Combine(localAppData, "DiscoverEgypt", "ExperienceStorage");
            }

            services.AddSingleton<IExperienceMediaStorage>(
                _ => new LocalExperienceMediaStorage(Path.Combine(rootPath, "media")));
            services.AddSingleton<IExperienceEvidenceStorage>(
                _ => new LocalExperienceEvidenceStorage(Path.Combine(rootPath, "evidence")));
        }
        else if (provider.Equals("Azure", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IExperienceMediaStorage>(_ => new AzureExperienceMediaStorage(
                configuration["ExperienceMediaStorage:ServiceUri"],
                configuration["ExperienceMediaStorage:ConnectionString"],
                configuration["ExperienceMediaStorage:ContainerName"]));

            services.AddSingleton<IExperienceEvidenceStorage>(_ => new AzureExperienceEvidenceStorage(
                configuration["ExperienceEvidenceStorage:ServiceUri"],
                configuration["ExperienceEvidenceStorage:ConnectionString"],
                configuration["ExperienceEvidenceStorage:ContainerName"]));
        }
        else
        {
            throw new InvalidOperationException(
                $"Unsupported ExperienceStorage:Provider '{provider}'. Use 'Local' or 'Azure'.");
        }

        return services;
    }
}
