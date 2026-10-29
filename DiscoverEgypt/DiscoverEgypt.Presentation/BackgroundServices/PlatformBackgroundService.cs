using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Bookings.Options;
using Microsoft.Extensions.Options;

namespace DiscoverEgypt.Presentation.BackgroundServices;

public sealed class PlatformBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<BookingLifecycleOptions> lifecycleOptions,
    TimeProvider timeProvider,
    ILogger<PlatformBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = lifecycleOptions.Value.LifecyclePollingInterval;
        if (interval <= TimeSpan.Zero) interval = TimeSpan.FromMinutes(1);

        using var timer = new PeriodicTimer(interval);
        do
        {
            await RunCycleAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RunCycleAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var lifecycleService = scope.ServiceProvider.GetRequiredService<IBookingLifecycleService>();
        var now = timeProvider.GetUtcNow().UtcDateTime;

        await RunStepAsync("cancel unanswered paid bookings", token => lifecycleService.CancelForNoResponseAsync(now, token), cancellationToken);
        await RunStepAsync("close unpaid bookings", token => lifecycleService.CancelUnpaidBookingsAsync(now, token), cancellationToken);
        await RunStepAsync("complete ended bookings", token => lifecycleService.AutoCompleteAsync(now, token), cancellationToken);
        await RunStepAsync("release eligible payouts", token => lifecycleService.ReleaseEligiblePayoutsAsync(now, token), cancellationToken);
        await RunStepAsync("expire guide licenses", token => lifecycleService.ExpireGuideLicensesAsync(now, token), cancellationToken);
    }

    private async Task RunStepAsync(
        string step,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken)
    {
        try
        {
            await operation(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Platform lifecycle step {LifecycleStep} failed. It will retry next cycle.", step);
        }
    }
}
