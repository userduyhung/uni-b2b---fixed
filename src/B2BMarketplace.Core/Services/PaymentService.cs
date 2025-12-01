using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for payment operations
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPremiumSubscriptionRepository _subscriptionRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly INotificationService _notificationService;
        private readonly IVerificationService _verificationService;

        /// <summary>
        /// Constructor for PaymentService
        /// </summary>
        /// <param name="paymentRepository">Payment repository</param>
        /// <param name="subscriptionRepository">Premium subscription repository</param>
        /// <param name="sellerProfileRepository">Seller profile repository</param>
        /// <param name="auditRepository">Audit repository</param>
        /// <param name="notificationService">Notification service</param>
        /// <param name="verificationService">Verification service</param>
        public PaymentService(
            IPaymentRepository paymentRepository,
            IPremiumSubscriptionRepository subscriptionRepository,
            ISellerProfileRepository sellerProfileRepository,
            IAuditRepository auditRepository,
            INotificationService notificationService,
            IVerificationService verificationService)
        {
            _paymentRepository = paymentRepository;
            _subscriptionRepository = subscriptionRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _auditRepository = auditRepository;
            _notificationService = notificationService;
            _verificationService = verificationService;
        }

        /// <summary>
        /// Initiates a payment for a seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="initiatePaymentDto">Payment details</param>
        /// <returns>Payment confirmation</returns>
        public async Task<PaymentConfirmationDto> InitiatePaymentAsync(Guid sellerProfileId, InitiatePaymentDto initiatePaymentDto)
        {
            // Create a payment record
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                SellerProfileId = sellerProfileId,
                Amount = initiatePaymentDto.Amount,
                Currency = initiatePaymentDto.Currency,
                PaymentProvider = "MockProvider", // In a real implementation, this would be determined by the payment method
                PaymentMethod = initiatePaymentDto.PaymentMethod,
                Description = initiatePaymentDto.Description,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var addedPayment = await _paymentRepository.AddPaymentAsync(payment);

            // In a real implementation, we would integrate with a payment provider here
            // For now, we'll simulate a successful payment processing

            // Simulate payment processing delay
            await Task.Delay(100);

            // For demonstration purposes, we'll assume the payment is successful
            // In a real implementation, this would be handled by a webhook from the payment provider
            var isSuccessful = true;
            var providerTransactionId = $"mock_txn_{Guid.NewGuid().ToString("N")[..16]}";

            // Process the payment confirmation
            await ProcessPaymentConfirmationAsync(addedPayment.Id, isSuccessful, providerTransactionId, null);

            return new PaymentConfirmationDto
            {
                PaymentId = addedPayment.Id,
                ProviderTransactionId = providerTransactionId,
                IsSuccessful = isSuccessful,
                ErrorMessage = null
            };
        }

        /// <summary>
        /// Processes a payment confirmation from the payment provider
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <param name="isSuccessful">Whether the payment was successful</param>
        /// <param name="providerTransactionId">Payment provider transaction ID</param>
        /// <param name="errorMessage">Error message if payment failed</param>
        /// <returns>True if processed successfully, false otherwise</returns>
        public async Task<bool> ProcessPaymentConfirmationAsync(Guid paymentId, bool isSuccessful, string? providerTransactionId, string? errorMessage)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return false;
            }

            // Update payment status
            payment.Status = isSuccessful ? PaymentStatus.Completed : PaymentStatus.Failed;
            payment.ProviderTransactionId = providerTransactionId;
            payment.CompletedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdatePaymentAsync(payment);

            // If payment was successful, activate premium features
            if (isSuccessful)
            {
                await ActivatePremiumFeaturesAsync(payment.SellerProfileId, payment.Id);

                // Log the successful payment in audit trail
                var paymentAuditLog = new UserManagementAuditLog();
                paymentAuditLog.InitializeAuditLog(
                    userId: payment.SellerProfile.UserId,
                    adminId: Guid.Empty, // No admin involved in automatic payment processing
                    action: "PaymentProcessed",
                    reason: $"Premium subscription payment of {payment.Amount} {payment.Currency} processed successfully",
                    entityName: "Payment",
                    entityId: payment.Id.GetHashCode() // Using hash code as integer ID for the GUID
                );

                await _auditRepository.LogUserManagementActionAsync(paymentAuditLog);

                // Notify the seller
                await _notificationService.CreateNotificationAsync(
                    payment.SellerProfile.UserId,
                    Core.DTOs.NotificationType.Payment,
                    "Payment Successful",
                    $"Your payment of {payment.Amount} {payment.Currency} has been processed successfully. Your premium features are now active.");
            }
            else
            {
                // Log the failed payment in audit trail
                var paymentAuditLog = new UserManagementAuditLog();
                paymentAuditLog.InitializeAuditLog(
                    userId: payment.SellerProfile.UserId,
                    adminId: Guid.Empty, // No admin involved in automatic payment processing
                    action: "PaymentFailed",
                    reason: $"Premium subscription payment of {payment.Amount} {payment.Currency} failed: {errorMessage}",
                    entityName: "Payment",
                    entityId: payment.Id.GetHashCode() // Using hash code as integer ID for the GUID
                );

                await _auditRepository.LogUserManagementActionAsync(paymentAuditLog);

                // Notify the seller
                await _notificationService.CreateNotificationAsync(
                    payment.SellerProfile.UserId,
                    Core.DTOs.NotificationType.Payment,
                    "Payment Failed",
                    $"Your payment of {payment.Amount} {payment.Currency} has failed. Please try again or contact support.");
            }

            return true;
        }

        /// <summary>
        /// Gets payments for a specific seller
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of payments</returns>
        public async Task<IEnumerable<PaymentDto>> GetPaymentsForSellerAsync(Guid sellerProfileId, int page, int pageSize)
        {
            var payments = await _paymentRepository.GetPaymentsForSellerAsync(sellerProfileId, page, pageSize);

            return payments.Select(payment => new PaymentDto
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Currency = payment.Currency,
                PaymentProvider = payment.PaymentProvider,
                Status = payment.Status.ToString(),
                PaymentMethod = payment.PaymentMethod,
                Description = payment.Description,
                CreatedAt = payment.CreatedAt,
                CompletedAt = payment.CompletedAt
            });
        }

        /// <summary>
        /// Processes a refund for a payment
        /// </summary>
        /// <param name="refundPaymentDto">Refund details</param>
        /// <param name="adminUserId">ID of the admin processing the refund</param>
        /// <returns>True if refunded successfully, false otherwise</returns>
        public async Task<bool> ProcessRefundAsync(RefundPaymentDto refundPaymentDto, Guid adminUserId)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(refundPaymentDto.PaymentId);
            if (payment == null)
            {
                return false;
            }

            // Check if payment can be refunded
            if (payment.Status != PaymentStatus.Completed)
            {
                return false;
            }

            // In a real implementation, we would integrate with the payment provider to process the refund
            // For now, we'll simulate a successful refund

            // Update payment status
            payment.Status = PaymentStatus.Refunded;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdatePaymentAsync(payment);

            // Deactivate premium features if this was a premium subscription payment
            // Find the subscription associated with this payment
            var subscription = await _subscriptionRepository.GetSubscriptionByPaymentIdAsync(payment.Id);
            if (subscription != null)
            {
                subscription.IsActive = false;
                subscription.EndDate = DateTime.UtcNow;
                subscription.UpdatedAt = DateTime.UtcNow;

                await _subscriptionRepository.UpdateSubscriptionAsync(subscription);

                // Update seller profile
                var sellerProfile = await _sellerProfileRepository.GetByIdAsync(subscription.SellerProfileId);
                if (sellerProfile != null)
                {
                    sellerProfile.IsPremium = false;
                    sellerProfile.UpdatedAt = DateTime.UtcNow;

                    await _sellerProfileRepository.UpdateAsync(sellerProfile);

                    // Update verification status based on new premium status
                    await _verificationService.UpdateVerificationStatusAsync(subscription.SellerProfileId);
                }
            }

            // Log the refund in audit trail
            var refundAuditLog = new UserManagementAuditLog();
            refundAuditLog.InitializeAuditLog(
                userId: payment.SellerProfile.UserId, // The user whose payment was refunded
                adminId: adminUserId, // The admin who processed the refund
                action: "PaymentRefunded",
                reason: $"Payment {payment.Id} refunded. Amount: {payment.Amount} {payment.Currency}. Reason: {refundPaymentDto.Reason}",
                entityName: "Payment",
                entityId: payment.Id.GetHashCode() // Using hash code as integer ID for the GUID
            );

            await _auditRepository.LogUserManagementActionAsync(refundAuditLog);

            // Notify the seller
            await _notificationService.CreateNotificationAsync(
                payment.SellerProfile.UserId,
                Core.DTOs.NotificationType.Payment,
                "Payment Refunded",
                $"Your payment of {payment.Amount} {payment.Currency} has been refunded. Reason: {refundPaymentDto.Reason}");

            return true;
        }

        /// <summary>
        /// Activates premium features for a seller based on successful payment
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>True if activated successfully, false otherwise</returns>
        public async Task<bool> ActivatePremiumFeaturesAsync(Guid sellerProfileId, Guid paymentId)
        {
            // Get seller profile
            var sellerProfile = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
            if (sellerProfile == null)
            {
                return false;
            }

            // Create a premium subscription
            var subscription = new PremiumSubscription
            {
                Id = Guid.NewGuid(),
                SellerProfileId = sellerProfileId,
                PlanType = "Premium",
                StartDate = DateTime.UtcNow,
                IsActive = true,
                IsAutoRenewing = true,
                PaymentId = paymentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _subscriptionRepository.AddSubscriptionAsync(subscription);

            // Update seller profile to mark as premium
            sellerProfile.IsPremium = true;
            sellerProfile.UpdatedAt = DateTime.UtcNow;

            await _sellerProfileRepository.UpdateAsync(sellerProfile);

            // Update verification status based on new premium status
            await _verificationService.UpdateVerificationStatusAsync(sellerProfileId);

            return true;
        }
    }
}