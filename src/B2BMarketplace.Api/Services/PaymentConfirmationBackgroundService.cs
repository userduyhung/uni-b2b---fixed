using B2BMarketplace.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace B2BMarketplace.Api.Services
{
    public class PaymentConfirmationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentConfirmationBackgroundService> _logger;
        private readonly TimeSpan _executionInterval = TimeSpan.FromMinutes(5); // Check every 5 minutes

        public PaymentConfirmationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<PaymentConfirmationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Payment Confirmation Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Payment Confirmation Background Service is processing.");

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var paymentConfirmationService = scope.ServiceProvider.GetRequiredService<IPaymentConfirmationService>();

                    await paymentConfirmationService.ProcessOutstandingPaymentsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing outstanding payments.");
                }

                _logger.LogInformation("Payment Confirmation Background Service is waiting for next execution.");

                try
                {
                    await Task.Delay(_executionInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Payment Confirmation Background Service was cancelled.");
                    break;
                }
            }

            _logger.LogInformation("Payment Confirmation Background Service is stopping.");
        }
    }
}