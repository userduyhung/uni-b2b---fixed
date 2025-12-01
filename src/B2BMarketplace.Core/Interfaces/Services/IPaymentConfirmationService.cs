using System;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface IPaymentConfirmationService
    {
        Task<bool> ProcessPaymentConfirmationAsync(Guid paymentId, string externalTransactionId);
        Task<bool> HandlePaymentWebhookAsync(string eventType, string transactionId, Guid paymentId, string status, decimal amount, string currency);
        Task ProcessOutstandingPaymentsAsync();
    }
}