using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.DTOs;
using Microsoft.Extensions.Logging;

namespace B2BMarketplace.Core.Services
{
    public class PremiumSubscriptionService : IPremiumSubscriptionService
    {
        private readonly IPremiumSubscriptionRepository _premiumSubscriptionRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryConfigurationService _categoryConfigurationService;
        private readonly ILogger<PremiumSubscriptionService> _logger;

        public PremiumSubscriptionService(
            IPremiumSubscriptionRepository premiumSubscriptionRepository,
            ISellerProfileRepository sellerProfileRepository,
            IUserRepository userRepository,
            ICategoryConfigurationService categoryConfigurationService,
            ILogger<PremiumSubscriptionService> logger)
        {
            _premiumSubscriptionRepository = premiumSubscriptionRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _userRepository = userRepository;
            _categoryConfigurationService = categoryConfigurationService;
            _logger = logger;
        }

        public async Task<bool> CreateSubscriptionFromPaymentAsync(Guid paymentId, Guid sellerId)
        {
            try
            {
                // Check if there's already an active subscription for this seller
                var activeSubscription = await _premiumSubscriptionRepository.GetActiveSubscriptionForSellerAsync(sellerId);
                if (activeSubscription != null)
                {
                    _logger.LogWarning($"Seller {sellerId} already has an active subscription");
                    return false;
                }

                // Create new premium subscription
                var subscription = new PremiumSubscription
                {
                    Id = Guid.NewGuid(),
                    SellerProfileId = sellerId,
                    PaymentId = paymentId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears(1), // Annual subscription
                    IsActive = true,
                    IsAutoRenewing = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _premiumSubscriptionRepository.AddSubscriptionAsync(subscription);

                // Update seller profile to premium status
                await UpdateSellerPremiumStatusAsync(sellerId, true);

                // Check if seller qualifies for verified badge
                await AssignVerifiedBadgeIfEligibleAsync(sellerId);

                _logger.LogInformation($"Premium subscription created for seller {sellerId} from payment {paymentId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating subscription from payment {paymentId} for seller {sellerId}");
                return false;
            }
        }

        public async Task<bool> ActivateSubscriptionAsync(Guid subscriptionId)
        {
            try
            {
                var subscription = await _premiumSubscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
                if (subscription == null)
                {
                    _logger.LogWarning($"Subscription with ID {subscriptionId} not found");
                    return false;
                }

                if (subscription.IsActive)
                {
                    _logger.LogInformation($"Subscription {subscriptionId} is already active");
                    return true;
                }

                // Activate the subscription
                subscription.IsActive = true;
                subscription.StartDate = DateTime.UtcNow;
                subscription.UpdatedAt = DateTime.UtcNow;

                // Set end date to one year from now if not already set
                if (subscription.EndDate <= DateTime.UtcNow)
                {
                    subscription.EndDate = DateTime.UtcNow.AddYears(1);
                }

                await _premiumSubscriptionRepository.UpdateSubscriptionAsync(subscription);

                // Update seller profile to premium status
                await UpdateSellerPremiumStatusAsync(subscription.SellerProfileId, true);

                // Check if seller qualifies for verified badge
                await AssignVerifiedBadgeIfEligibleAsync(subscription.SellerProfileId);

                _logger.LogInformation($"Subscription {subscriptionId} activated for seller {subscription.SellerProfileId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating subscription {subscriptionId}");
                return false;
            }
        }

        public async Task<PremiumSubscriptionDto?> GetActiveSubscriptionForSellerAsync(Guid sellerId)
        {
            try
            {
                var subscription = await _premiumSubscriptionRepository.GetActiveSubscriptionForSellerAsync(sellerId);
                if (subscription == null)
                {
                    return null;
                }

                return new PremiumSubscriptionDto
                {
                    Id = subscription.Id,
                    SellerId = subscription.SellerProfileId,
                    PaymentId = subscription.PaymentId ?? Guid.Empty,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate ?? DateTime.UtcNow.AddYears(1),
                    Status = subscription.IsActive ? "Active" : "Inactive",
                    CreatedDate = subscription.CreatedAt,
                    UpdatedDate = subscription.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting active subscription for seller {sellerId}");
                return null;
            }
        }

        public async Task<bool> UpdateSellerPremiumStatusAsync(Guid sellerId, bool isPremium)
        {
            try
            {
                var sellerProfile = await _sellerProfileRepository.GetByUserIdAsync(sellerId);
                if (sellerProfile == null)
                {
                    _logger.LogWarning($"Seller profile not found for user ID {sellerId}");
                    return false;
                }

                sellerProfile.IsPremium = isPremium;
                if (isPremium)
                {
                    sellerProfile.PremiumSince = DateTime.UtcNow;
                }
                else
                {
                    sellerProfile.PremiumSince = null;
                }
                sellerProfile.UpdatedAt = DateTime.UtcNow;

                await _sellerProfileRepository.UpdateAsync(sellerProfile);

                _logger.LogInformation($"Premium status updated for seller {sellerId}: {isPremium}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating premium status for seller {sellerId}");
                return false;
            }
        }

        public async Task<bool> AssignVerifiedBadgeIfEligibleAsync(Guid sellerId)
        {
            try
            {
                var sellerProfile = await _sellerProfileRepository.GetByUserIdAsync(sellerId);
                if (sellerProfile == null)
                {
                    _logger.LogWarning($"Seller profile not found for user ID {sellerId}");
                    return false;
                }

                // Check if seller meets certification requirements for verified badge
                bool hasVerifiedBadge = false;
                if (sellerProfile.PrimaryCategoryId.HasValue)
                {
                    hasVerifiedBadge = await _categoryConfigurationService.CanSellerReceiveBadgeAsync(sellerProfile.Id, sellerProfile.PrimaryCategoryId.Value);
                }
                else
                {
                    // If no primary category is set, seller cannot receive verified badge
                    _logger.LogInformation($"Seller {sellerProfile.Id} does not have a primary category, verified badge not assigned.");
                }

                sellerProfile.HasVerifiedBadge = hasVerifiedBadge;
                sellerProfile.UpdatedAt = DateTime.UtcNow;

                await _sellerProfileRepository.UpdateAsync(sellerProfile);

                _logger.LogInformation($"Verified badge eligibility assessed for seller {sellerId}. Eligible: {hasVerifiedBadge}");
                return hasVerifiedBadge;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning verified badge for seller {sellerId}");
                return false;
            }
        }

        public async Task<IEnumerable<PremiumSubscriptionDto>> GetSubscriptionsBySellerAsync(Guid sellerId)
        {
            try
            {
                var subscriptions = await _premiumSubscriptionRepository.GetSubscriptionsForSellerAsync(sellerId, 1, 100); // Using a high page size to get all

                return subscriptions.Select(s => new PremiumSubscriptionDto
                {
                    Id = s.Id,
                    SellerId = s.SellerProfileId,
                    PaymentId = s.PaymentId ?? Guid.Empty,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate ?? DateTime.UtcNow.AddYears(1),
                    Status = s.IsActive ? "Active" : "Inactive",
                    CreatedDate = s.CreatedAt,
                    UpdatedDate = s.UpdatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subscriptions for seller {sellerId}");
                return new List<PremiumSubscriptionDto>();
            }
        }

        public async Task<PremiumSubscriptionDto?> GetSubscriptionByIdAsync(Guid subscriptionId)
        {
            try
            {
                var subscription = await _premiumSubscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
                if (subscription == null)
                {
                    return null;
                }

                return new PremiumSubscriptionDto
                {
                    Id = subscription.Id,
                    SellerId = subscription.SellerProfileId,
                    PaymentId = subscription.PaymentId ?? Guid.Empty,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate ?? DateTime.UtcNow.AddYears(1),
                    Status = subscription.IsActive ? "Active" : "Inactive",
                    CreatedDate = subscription.CreatedAt,
                    UpdatedDate = subscription.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subscription with ID {subscriptionId}");
                return null;
            }
        }

        public async Task<bool> HasActivePremiumSubscriptionAsync(Guid sellerId)
        {
            try
            {
                var subscription = await _premiumSubscriptionRepository.GetActiveSubscriptionForSellerAsync(sellerId);
                return subscription != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking active subscription for seller {sellerId}");
                return false;
            }
        }
    }
}