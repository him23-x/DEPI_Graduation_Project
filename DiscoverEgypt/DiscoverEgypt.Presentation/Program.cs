using DiscoverEgypt.Application;
using DiscoverEgypt.Infrastructure;
using DiscoverEgypt.Persistence;
using DiscoverEgypt.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.ConfigurePersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPresentationServices();

var app = builder.Build();

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
    name: "host-experiences",
    pattern: "Host/Experiences/{action=Index}/{id?}",
    defaults: new { controller = "Experiences" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Experiences}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
