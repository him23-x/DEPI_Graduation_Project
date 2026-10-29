using DiscoverEgypt.Application;
using DiscoverEgypt.Infrastructure;
using DiscoverEgypt.Persistence;
using DiscoverEgypt.Presentation;
using DiscoverEgypt.Persistence.Identity;
using DiscoverEgypt.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.ConfigurePersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddPresentationServices();

var app = builder.Build();

await IdentityRoleSeeder.SeedAsync(app.Services);
if (app.Environment.IsDevelopment())
{
    await DevelopmentDataSeeder.SeedAsync(
        app.Services,
        builder.Configuration["DevelopmentSeed:DemoPassword"] ?? string.Empty);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("An unexpected error occurred.");
    }));
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "host-bookings",
    pattern: "Host/Bookings/{action=Index}/{id?}",
    defaults: new { controller = "Bookings" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "host-earnings",
    pattern: "Host/Earnings/{action=Index}",
    defaults: new { controller = "Earnings" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "host-dashboard",
    pattern: "Host/Dashboard/{action=Index}",
    defaults: new { controller = "Dashboard" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "host-experiences",
    pattern: "Host/Experiences/{action=Index}/{id?}",
    defaults: new { controller = "Experiences" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Experiences}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
