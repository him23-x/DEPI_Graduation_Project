using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Notifications;
using DiscoverEgypt.Application.Features.Bookings.Options;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Persistence.Services.Bookings;
using DiscoverEgypt.Persistence.Services.Notifications;
using DiscoverEgypt.Persistence.Repositories.Host.Bookings;
using DiscoverEgypt.Persistence.Repositories.Host.Earnings;
using DiscoverEgypt.Persistence.Repositories.Host.Dashboard;
using DiscoverEgypt.Persistence.Repositories.Host.Reviews;
using DiscoverEgypt.Persistence.Repositories.Host.Disputes;
using DiscoverEgypt.Persistence.Repositories.Host.Experiences;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
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

        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        });
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
        });

        services.AddScoped<IHostExperienceRepository, HostExperienceRepository>();
        services.AddScoped<IExperienceStatusRepository, HostExperienceStatusRepository>();
        services.AddScoped<IHostExperienceMediaRepository, HostExperienceMediaRepository>();
        services.AddScoped<IHostExperienceEvidenceRepository, HostExperienceEvidenceRepository>();
        services.AddScoped<IHostExperienceSubmissionRepository, HostExperienceSubmissionRepository>();
        services.AddScoped<IHostBookingRepository, HostBookingRepository>();
        services.AddScoped<IHostEarningsRepository, HostEarningsRepository>();
        services.AddScoped<IHostDashboardRepository, HostDashboardRepository>();
        services.AddScoped<IHostReviewsRepository, HostReviewsRepository>();
        services.AddScoped<IHostDisputesRepository, HostDisputesRepository>();
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
