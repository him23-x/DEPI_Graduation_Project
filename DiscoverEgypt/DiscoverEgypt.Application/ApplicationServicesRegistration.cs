using AutoMapper;
using DiscoverEgypt.Application.Common.Behaviors;
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
        services.AddTransient<Features.HostExperiences.Requests.Validation.HostExperienceSubmissionReviewValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
