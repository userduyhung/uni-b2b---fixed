using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Api.Services
{
    /// <summary>
    /// Background service for processing scheduled content publishing and unpublishing
    /// </summary>
    public class ScheduledContentProcessingService : BackgroundService
    {
        private readonly ILogger<ScheduledContentProcessingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;

        /// <summary>
        /// Constructor for ScheduledContentProcessingService
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="serviceProvider">Service provider for dependency injection</param>
        public ScheduledContentProcessingService(
            ILogger<ScheduledContentProcessingService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Main execution method for the background service
        /// </summary>
        /// <param name="stoppingToken">Cancellation token</param>
        /// <returns>Task</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Content Processing Service started at: {Time}", DateTimeOffset.Now);

            // Set up a timer to run every minute
            _timer = new Timer(async (state) => await ProcessScheduledContent(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            // Keep the service running until cancellation is requested
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken); // Check every second for cancellation
            }
        }

        /// <summary>
        /// Processes scheduled content for publishing and unpublishing
        /// </summary>
        /// <returns>Task</returns>
        private async Task ProcessScheduledContent()
        {
            try
            {
                _logger.LogInformation("Processing scheduled content at: {Time}", DateTimeOffset.Now);

                // Create a scope to resolve scoped services
                using var scope = _serviceProvider.CreateScope();
                var contentItemService = scope.ServiceProvider.GetRequiredService<IContentItemService>();

                await contentItemService.ProcessScheduledContentAsync();

                _logger.LogInformation("Scheduled content processing completed at: {Time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing scheduled content at: {Time}", DateTimeOffset.Now);
            }
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown
        /// </summary>
        /// <param name="stoppingToken">Cancellation token</param>
        /// <returns>Task</returns>
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Content Processing Service is stopping at: {Time}", DateTimeOffset.Now);

            _timer?.Change(Timeout.Infinite, 0);

            await base.StopAsync(stoppingToken);

            _logger.LogInformation("Scheduled Content Processing Service has stopped at: {Time}", DateTimeOffset.Now);
        }

        /// <summary>
        /// Disposes of the timer
        /// </summary>
        /// <param name="disposing">Whether disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Dispose();
            }

            base.Dispose();
        }
    }
}