using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using DiscoverEgypt.Presentation.BackgroundServices;
using DiscoverEgypt.Presentation.Mapping;

namespace DiscoverEgypt.Presentation;

public static class PresentationServicesRegistration
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        services.AddControllersWithViews().AddRazorOptions(options =>
            options.ViewLocationFormats.Insert(0, "/Views/Host/{1}/{0}.cshtml"));
        services.AddAuthorization();
        services.AddAutoMapper(configuration =>
        {
            configuration.LicenseKey = Environment.GetEnvironmentVariable("AUTOMAPPER_LICENSE_KEY");
        }, typeof(PresentationMappingProfile));
        services.AddHostedService<PlatformBackgroundService>();

        return services;
    }
}
