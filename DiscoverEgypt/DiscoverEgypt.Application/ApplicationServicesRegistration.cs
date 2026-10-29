using AutoMapper;
using DiscoverEgypt.Application.Common.Behaviors;
using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiscoverEgypt.Application;

public static class ApplicationServicesRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var applicationAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(applicationAssembly);
        services.AddAutoMapper(configuration =>
        {
            configuration.LicenseKey = Environment.GetEnvironmentVariable("AUTOMAPPER_LICENSE_KEY");
        }, applicationAssembly);
        services.AddValidatorsFromAssembly(applicationAssembly, ServiceLifetime.Transient);
        services.AddTransient<Features.HostExperiences.Validators.DTOs.HostExperienceSubmissionReviewValidator>();
        services.AddScoped<IExperienceReadinessService, ExperienceReadinessService>();
        services.AddScoped<IExperienceStatusService, ExperienceStatusService>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
