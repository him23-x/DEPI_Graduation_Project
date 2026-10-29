using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Notifications;
using DiscoverEgypt.Application.Features.Bookings.Options;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Persistence.Services;
using DiscoverEgypt.Persistence.Repositories.Host;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

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
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IBookingLifecycleService, BookingLifecycleService>();
        services.Configure<BookingLifecycleOptions>(options =>
        {
            options.ProviderResponseTimeout = ReadDuration(
                configuration["BookingLifecycle:ProviderResponseTimeout"], options.ProviderResponseTimeout,
                "BookingLifecycle:ProviderResponseTimeout");
            options.UnpaidBookingTimeout = ReadDuration(
                configuration["BookingLifecycle:UnpaidBookingTimeout"], options.UnpaidBookingTimeout,
                "BookingLifecycle:UnpaidBookingTimeout");
            options.LifecyclePollingInterval = ReadDuration(
                configuration["BookingLifecycle:LifecyclePollingInterval"], options.LifecyclePollingInterval,
                "BookingLifecycle:LifecyclePollingInterval");
        });
        services.AddSingleton(TimeProvider.System);

        return services;
    }

    private static TimeSpan ReadDuration(string? value, TimeSpan fallback, string settingName)
    {
        if (string.IsNullOrWhiteSpace(value)) return fallback;
        if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var duration) && duration > TimeSpan.Zero)
            return duration;

        throw new InvalidOperationException($"Configuration value '{settingName}' must be a positive duration such as 12:00:00.");
    }
}
