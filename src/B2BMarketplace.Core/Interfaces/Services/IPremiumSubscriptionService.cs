using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface IPremiumSubscriptionService
    {
        Task<bool> CreateSubscriptionFromPaymentAsync(Guid paymentId, Guid sellerId);
        Task<bool> ActivateSubscriptionAsync(Guid subscriptionId);
        Task<PremiumSubscriptionDto?> GetActiveSubscriptionForSellerAsync(Guid sellerId);
        Task<bool> UpdateSellerPremiumStatusAsync(Guid sellerId, bool isPremium);
        Task<bool> AssignVerifiedBadgeIfEligibleAsync(Guid sellerId);
        Task<IEnumerable<PremiumSubscriptionDto>> GetSubscriptionsBySellerAsync(Guid sellerId);
        Task<PremiumSubscriptionDto?> GetSubscriptionByIdAsync(Guid subscriptionId);
        Task<bool> HasActivePremiumSubscriptionAsync(Guid sellerId);
    }
}