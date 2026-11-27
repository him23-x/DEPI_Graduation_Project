using AutoMapper;
using DiscoverEgypt.Application.Contracts.HostExperiences;
using DiscoverEgypt.Application.Features.HostExperiences.Services;
using DiscoverEgypt.Application.Features.Bookings.Services;
using DiscoverEgypt.Application.Features.BookingShares.Services;
using DiscoverEgypt.Application.Features.Disputes.Services;
using DiscoverEgypt.Application.Features.Reviews.Services;
using DiscoverEgypt.Application.Features.Payments.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiscoverEgypt.Application
{
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
            services.AddTransient<Features.HostExperiences.Validators.DTOs.HostExperienceSubmissionReviewDtoValidator>();
            services.AddScoped<IExperienceReadinessService, ExperienceReadinessService>();
            services.AddScoped<ExperienceBookingService>();
            services.AddScoped<GuideBookingService>();
            services.AddScoped<MockPaymentService>();
            services.AddScoped<TouristBookingService>();
            services.AddScoped<TouristDisputeService>();
            services.AddScoped<TouristReviewService>();
            services.AddScoped<BookingShareService>();
            return services;
        }
    }
}

