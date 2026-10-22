using DiscoverEgypt.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoverEgypt.Persistence
{
    public static class PersistenceConfiguration
    {
        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<DiscoverEgyptDbContext>(options =>
            options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")));

            services
           .AddIdentity<ApplicationUser, IdentityRole>()
           .AddEntityFrameworkStores<DiscoverEgyptDbContext>()
           .AddDefaultTokenProviders();


            return services;
        }
    }
}
