using System;

namespace B2BMarketplace.Core.DTOs
{


    public class PaymentWebhookDto
    {
        public string Event { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public Guid PaymentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
    }

    public class PaymentConfirmationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaymentConfirmationResultDto Data { get; set; } = new PaymentConfirmationResultDto();
        public DateTime Timestamp { get; set; }
    }

    public class PaymentConfirmationResultDto
    {
        public Guid PaymentId { get; set; }
        public Guid SellerId { get; set; }
        public bool PremiumStatusUpdated { get; set; }
        public bool SubscriptionActivated { get; set; }
        public bool VerifiedBadgeAssigned { get; set; }
    }
}