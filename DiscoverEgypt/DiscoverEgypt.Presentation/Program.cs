using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Application;
using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Contracts.Storage;
using DiscoverEgypt.Persistence;
using DiscoverEgypt.Persistence.Repositories;
using DiscoverEgypt.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorOptions(options =>
    options.ViewLocationFormats.Insert(0, "/Views/Host/{1}/{0}.cshtml"));
builder.Services.AddApplicationServices();

builder.Services.AddDbContext<DiscoverEgyptDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DiscoverEgyptDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();
builder.Services.AddScoped<IHostExperienceRepository, HostExperienceRepository>();
builder.Services.AddScoped<IHostExperienceMediaRepository, HostExperienceMediaRepository>();
builder.Services.AddScoped<IHostExperienceEvidenceRepository, HostExperienceEvidenceRepository>();
builder.Services.AddScoped<IHostExperienceSubmissionRepository, HostExperienceSubmissionRepository>();
builder.Services.AddSingleton<IExperienceMediaStorage>(_ => new AzureExperienceMediaStorage(
    builder.Configuration["ExperienceMediaStorage:ServiceUri"],
    builder.Configuration["ExperienceMediaStorage:ConnectionString"],
    builder.Configuration["ExperienceMediaStorage:ContainerName"]));
builder.Services.AddSingleton<IExperienceEvidenceStorage>(_ => new AzureExperienceEvidenceStorage(
    builder.Configuration["ExperienceEvidenceStorage:ServiceUri"],
    builder.Configuration["ExperienceEvidenceStorage:ConnectionString"],
    builder.Configuration["ExperienceEvidenceStorage:ContainerName"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "host-experiences",
    pattern: "Host/Experiences/{action=Index}/{id?}",
    defaults: new { controller = "Experiences" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
