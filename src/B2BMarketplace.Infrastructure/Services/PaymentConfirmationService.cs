using System;
using System.Threading.Tasks;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Entities;
using Microsoft.Extensions.Logging;
using B2BMarketplace.Infrastructure.Data;

namespace B2BMarketplace.Infrastructure.Services
{
    public class PaymentConfirmationService : IPaymentConfirmationService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPremiumSubscriptionRepository _premiumSubscriptionRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly ICategoryConfigurationService _categoryConfigurationService;
        private readonly ILogger<PaymentConfirmationService> _logger;

        public PaymentConfirmationService(
            IPaymentRepository paymentRepository,
            IPremiumSubscriptionRepository premiumSubscriptionRepository,
            ISellerProfileRepository sellerProfileRepository,
            ICategoryConfigurationService categoryConfigurationService,
            ILogger<PaymentConfirmationService> logger)
        {
            _paymentRepository = paymentRepository;
            _premiumSubscriptionRepository = premiumSubscriptionRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _categoryConfigurationService = categoryConfigurationService;
            _logger = logger;
        }

        public async Task<bool> ProcessPaymentConfirmationAsync(Guid paymentId, string externalTransactionId)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    _logger.LogWarning($"Payment with ID {paymentId} not found");
                    return false;
                }

                if (payment.Status == PaymentStatus.Completed)
                {
                    _logger.LogInformation($"Payment {paymentId} is already marked as completed");
                    return await UpdatePremiumStatusAsync(payment.SellerProfileId, paymentId);
                }

                // Mark payment as completed
                payment.Status = PaymentStatus.Completed;
                payment.ProviderTransactionId = externalTransactionId;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);

                // Update premium status
                return await UpdatePremiumStatusAsync(payment.SellerProfileId, paymentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing payment confirmation for payment ID {paymentId}");
                return false;
            }
        }

        public async Task<bool> HandlePaymentWebhookAsync(string eventType, string transactionId, Guid paymentId, string status, decimal amount, string currency)
        {
            try
            {
                if (eventType != "payment.completed")
                {
                    _logger.LogInformation($"Ignoring non-completion event: {eventType}");
                    return true;
                }

                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    _logger.LogWarning($"Payment with ID {paymentId} not found for webhook processing");
                    return false;
                }

                // Update payment status based on webhook
                payment.Status = Enum.Parse<PaymentStatus>(status);
                payment.ProviderTransactionId = transactionId;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);

                if (status == "Completed")
                {
                    return await UpdatePremiumStatusAsync(payment.SellerProfileId, paymentId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling payment webhook for payment ID {paymentId}");
                return false;
            }
        }

        public async Task ProcessOutstandingPaymentsAsync()
        {
            try
            {
                // Get all payments with pending status that were created more than 5 minutes ago
                // This is to catch payments that might have missed the webhook
                var pendingPayments = await _paymentRepository.GetPendingPaymentsAsync();

                foreach (var payment in pendingPayments)
                {
                    // In a real implementation, you would call the payment provider's API
                    // to verify if the payment has been completed
                    // For now, this is a placeholder implementation
                    _logger.LogInformation($"Checking status for payment {payment.Id}");

                    // In a real implementation, you would call payment provider API to verify status
                    // For this placeholder, we'll skip updating since we don't have actual provider integration
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outstanding payments");
            }
        }

        private async Task<bool> UpdatePremiumStatusAsync(Guid sellerId, Guid paymentId)
        {
            try
            {
                // Create/activate premium subscription
                var subscription = new PremiumSubscription
                {
                    Id = Guid.NewGuid(),
                    SellerProfileId = sellerId,
                    PaymentId = paymentId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears(1), // Assuming annual premium subscription
                    IsActive = true
                };

                await _premiumSubscriptionRepository.AddAsync(subscription);

                // Update seller profile to mark as premium
                var sellerProfile = await _sellerProfileRepository.GetByUserIdAsync(sellerId);
                if (sellerProfile != null)
                {
                    sellerProfile.IsPremium = true;
                    sellerProfile.PremiumSince = DateTime.UtcNow;
                    sellerProfile.UpdatedAt = DateTime.UtcNow;

                    // Check if seller meets certification requirements for verified badge
                    // This checks if seller has all required certifications for their primary category
                    bool hasVerifiedBadge = false;
                    if (sellerProfile.PrimaryCategoryId.HasValue)
                    {
                        hasVerifiedBadge = await CheckSellerCertificationRequirementsAsync(sellerProfile.Id, sellerProfile.PrimaryCategoryId.Value);
                    }
                    else
                    {
                        // If no primary category is set, seller cannot receive verified badge
                        _logger.LogInformation($"Seller {sellerProfile.Id} does not have a primary category, verified badge not assigned.");
                    }

                    sellerProfile.HasVerifiedBadge = hasVerifiedBadge;
                    await _sellerProfileRepository.UpdateAsync(sellerProfile);

                    _logger.LogInformation($"Premium status updated for seller {sellerId}, Verified Badge Status: {hasVerifiedBadge}");
                    return true;
                }

                _logger.LogWarning($"Seller profile not found for user ID {sellerId}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating premium status for seller {sellerId}");
                return false;
            }
        }

        private async Task<bool> CheckSellerCertificationRequirementsAsync(Guid sellerProfileId, Guid categoryId)
        {
            try
            {
                // Use the CategoryConfigurationService to check if the seller meets certification requirements
                return await _categoryConfigurationService.CanSellerReceiveBadgeAsync(sellerProfileId, categoryId);
            }
            catch
            {
                // If there's an error checking certification requirements, default to false
                _logger.LogWarning($"Error checking certification requirements for seller {sellerProfileId} and category {categoryId}");
                return false;
            }
        }
    }
}