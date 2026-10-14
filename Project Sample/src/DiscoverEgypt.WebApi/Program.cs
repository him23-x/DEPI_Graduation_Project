using System.Globalization;
using DiscoverEgypt.Application.Abstractions;
using DiscoverEgypt.Application.Bookings;
using DiscoverEgypt.Application.Common;
using DiscoverEgypt.Application.Experiences;
using DiscoverEgypt.Domain.Common;
using DiscoverEgypt.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ---- Composition root -------------------------------------------------------
// This is the ONLY place that knows the concrete classes.
// It connects each Interface (defined in Application) to its implementation (Infrastructure).

// Fee rate: fail fast if it is missing (as the Project Context requires).
// The value in appsettings.json (0.15) is illustrative, not final.
var feeText = builder.Configuration["PlatformFee:ExperienceRate"];
if (!decimal.TryParse(feeText, NumberStyles.Number, CultureInfo.InvariantCulture, out var experienceFeeRate))
    throw new InvalidOperationException("PlatformFee:ExperienceRate is missing or invalid in configuration.");
builder.Services.AddSingleton(new PlatformFeeSettings { ExperienceRate = experienceFeeRate });

// Singleton ONLY because the in-memory data must survive between requests.
// With Entity Framework Core these become AddScoped.
builder.Services.AddSingleton<IExperienceRepository, InMemoryExperienceRepository>();
builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();

builder.Services.AddScoped<ExperienceService>();
builder.Services.AddScoped<BookingService>();

// Only needed if the frontend runs on a DIFFERENT address (for example Live Server on port 5500).
// In this sample the frontend is served from wwwroot (same address), so CORS is not required.
// builder.Services.AddCors(o => o.AddPolicy("Frontend", p =>
//     p.WithOrigins("http://localhost:5500").AllowAnyHeader().AllowAnyMethod()));
// ------------------------------------------------------------------------------

var app = builder.Build();

// Translate exceptions from inner layers to HTTP responses.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DomainException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (NotFoundException ex)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

// app.UseCors("Frontend");   // uncomment together with AddCors above

app.UseDefaultFiles();   // serves wwwroot/index.html at "/"
app.UseStaticFiles();
app.MapControllers();

app.Run();
