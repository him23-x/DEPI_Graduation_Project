using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Persistence.Repositories.Host;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscoverEgypt.Persistence;

public static class PersistenceConfiguration
{
    public static IServiceCollection ConfigurePersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<DiscoverEgyptDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<DiscoverEgyptDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IHostExperienceRepository, HostExperienceRepository>();
        services.AddScoped<IHostExperienceMediaRepository, HostExperienceMediaRepository>();
        services.AddScoped<IHostExperienceEvidenceRepository, HostExperienceEvidenceRepository>();
        services.AddScoped<IHostExperienceSubmissionRepository, HostExperienceSubmissionRepository>();
        services.AddScoped<IHostBookingRepository, HostBookingRepository>();

        return services;
    }
}
