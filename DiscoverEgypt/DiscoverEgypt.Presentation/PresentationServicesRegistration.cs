using Microsoft.Extensions.DependencyInjection;
using DiscoverEgypt.Presentation.BackgroundServices;

namespace DiscoverEgypt.Presentation;

public static class PresentationServicesRegistration
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        services.AddControllersWithViews().AddRazorOptions(options =>
            options.ViewLocationFormats.Insert(0, "/Views/Host/{1}/{0}.cshtml"));
        services.AddAuthorization();
        services.AddHostedService<PlatformBackgroundService>();

        return services;
    }
}
