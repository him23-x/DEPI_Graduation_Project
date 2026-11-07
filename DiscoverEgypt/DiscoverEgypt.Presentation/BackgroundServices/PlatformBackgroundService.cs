using DiscoverEgypt.Application.Contracts.Persistence;
using DiscoverEgypt.Application.Features.Bookings.Options;
using Microsoft.Extensions.Options;

namespace DiscoverEgypt.Presentation.BackgroundServices
{
    public sealed class PlatformBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<BookingLifecycleOptions> _lifecycleOptions;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<PlatformBackgroundService> _logger;

        public PlatformBackgroundService(
            IServiceScopeFactory scopeFactory,
            IOptions<BookingLifecycleOptions> lifecycleOptions,
            TimeProvider timeProvider,
            ILogger<PlatformBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _lifecycleOptions = lifecycleOptions;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = _lifecycleOptions.Value.LifecyclePollingInterval;
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
            await using var scope = _scopeFactory.CreateAsyncScope();
            var lifecycleService = scope.ServiceProvider.GetRequiredService<IBookingLifecycleService>();
            var now = _timeProvider.GetUtcNow().UtcDateTime;

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
                _logger.LogError(exception, "Platform lifecycle step {LifecycleStep} failed. It will retry next cycle.", step);
            }
        }
    }
}

