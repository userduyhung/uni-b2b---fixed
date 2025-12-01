using System.Linq;
using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace B2BMarketplace.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationPreferencesRepository _notificationPreferencesRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;
        private readonly IBuyerProfileRepository _buyerProfileRepository;
        private readonly IRFQRepository _rfqRepository;

        public NotificationService(
            ILogger<NotificationService> logger,
            INotificationRepository notificationRepository,
            INotificationPreferencesRepository notificationPreferencesRepository,
            IUserRepository userRepository,
            ISellerProfileRepository sellerProfileRepository,
            IBuyerProfileRepository buyerProfileRepository,
            IRFQRepository rfqRepository)
        {
            _logger = logger;
            _notificationRepository = notificationRepository;
            _notificationPreferencesRepository = notificationPreferencesRepository;
            _userRepository = userRepository;
            _sellerProfileRepository = sellerProfileRepository;
            _buyerProfileRepository = buyerProfileRepository;
            _rfqRepository = rfqRepository;
        }

        public async Task SendQuoteSubmittedNotificationAsync(Quote quote)
        {
            // Get the buyer profile associated with the RFQ
            var rfq = quote.RFQ;
            if (rfq == null)
            {
                rfq = await _rfqRepository.GetByIdAsync(quote.RFQId);
                if (rfq == null)
                {
                    _logger.LogWarning("RFQ with ID {RFQId} not found for quote {QuoteId}", quote.RFQId, quote.Id);
                    return;
                }
            }

            var buyerProfile = rfq.BuyerProfile;
            if (buyerProfile == null)
            {
                buyerProfile = await _buyerProfileRepository.GetByUserIdAsync(rfq.BuyerProfileId);
                if (buyerProfile == null)
                {
                    _logger.LogWarning("Buyer profile with ID {BuyerProfileId} not found for RFQ {RFQId}", rfq.BuyerProfileId, rfq.Id);
                    return;
                }
            }

            var user = buyerProfile.User;
            if (user == null)
            {
                user = await _userRepository.GetUserByIdAsync(buyerProfile.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for buyer profile {BuyerProfileId}", buyerProfile.UserId, buyerProfile.Id);
                    return;
                }
            }

            // Get notification preferences
            var preferences = await _notificationPreferencesRepository.GetByUserIdAsync(user.Id);
            if (preferences == null || !preferences.RFQResponseNotifications)
            {
                return; // Skip notification if user has disabled this type
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = B2BMarketplace.Core.Entities.NotificationType.RFQResponse,
                Title = "New Quote Received",
                Message = $"Seller has responded to your RFQ '{rfq.Title}' with a quote of {quote.Price:C}",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                RelatedEntityId = quote.Id,
                RelatedEntityType = "Quote"
            };

            await _notificationRepository.CreateAsync(notification);

            // In a real implementation, this would send an email to the buyer
            _logger.LogInformation("RFQ response notification created for user {UserId} for quote {QuoteId}",
                user.Id, quote.Id);
        }

        public async Task SendRFQResponseNotificationAsync(RFQ rfq, Quote quote)
        {
            // This is the same as SendQuoteSubmittedNotificationAsync
            await SendQuoteSubmittedNotificationAsync(quote);
        }

        public async Task SendRFQReceivedNotificationAsync(RFQ rfq, Guid sellerProfileId)
        {
            var sellerProfile = await _sellerProfileRepository.GetByIdAsync(sellerProfileId);
            if (sellerProfile == null)
            {
                _logger.LogWarning("Seller profile with ID {SellerProfileId} not found", sellerProfileId);
                return;
            }

            var user = await _userRepository.GetUserByIdAsync(sellerProfile.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for seller profile {SellerProfileId}", sellerProfile.UserId, sellerProfileId);
                return;
            }

            // Get notification preferences
            var preferences = await _notificationPreferencesRepository.GetByUserIdAsync(user.Id);
            if (preferences == null || !preferences.RFQReceivedNotifications)
            {
                return; // Skip notification if user has disabled this type
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = B2BMarketplace.Core.Entities.NotificationType.RFQReceived,
                Title = "New RFQ Received",
                Message = $"You have received a new RFQ: {rfq.Title}",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                RelatedEntityId = rfq.Id,
                RelatedEntityType = "RFQ"
            };

            await _notificationRepository.CreateAsync(notification);

            // In a real implementation, this would send an email to the seller
            _logger.LogInformation("RFQ received notification created for seller {SellerProfileId} for RFQ {RFQId}",
                sellerProfileId, rfq.Id);
        }

        public async Task SendQuoteUpdatedNotificationAsync(Quote quote)
        {
            // Get the buyer profile associated with the RFQ
            var rfq = quote.RFQ;
            if (rfq == null)
            {
                rfq = await _rfqRepository.GetByIdAsync(quote.RFQId);
                if (rfq == null)
                {
                    _logger.LogWarning("RFQ with ID {RFQId} not found for quote {QuoteId}", quote.RFQId, quote.Id);
                    return;
                }
            }

            var buyerProfile = rfq.BuyerProfile;
            if (buyerProfile == null)
            {
                buyerProfile = await _buyerProfileRepository.GetByUserIdAsync(rfq.BuyerProfileId);
                if (buyerProfile == null)
                {
                    _logger.LogWarning("Buyer profile with ID {BuyerProfileId} not found for RFQ {RFQId}", rfq.BuyerProfileId, rfq.Id);
                    return;
                }
            }

            var user = buyerProfile.User;
            if (user == null)
            {
                user = await _userRepository.GetUserByIdAsync(buyerProfile.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for buyer profile {BuyerProfileId}", buyerProfile.UserId, buyerProfile.Id);
                    return;
                }
            }

            // Get notification preferences
            var preferences = await _notificationPreferencesRepository.GetByUserIdAsync(user.Id);
            if (preferences == null || !preferences.QuoteUpdatedNotifications)
            {
                return; // Skip notification if user has disabled this type
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = B2BMarketplace.Core.Entities.NotificationType.QuoteUpdated,
                Title = "Quote Updated",
                Message = $"A quote for your RFQ '{rfq.Title}' has been updated",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                RelatedEntityId = quote.Id,
                RelatedEntityType = "Quote"
            };

            await _notificationRepository.CreateAsync(notification);

            // In a real implementation, this would send an email to the buyer
            _logger.LogInformation("Quote updated notification created for user {UserId} for quote {QuoteId}",
                user.Id, quote.Id);
        }

        public async Task SendCertificationStatusNotificationAsync(Certification certification)
        {
            var sellerProfile = await _sellerProfileRepository.GetByUserIdAsync(certification.SellerProfileId);
            if (sellerProfile == null)
            {
                _logger.LogWarning("Seller profile with ID {SellerProfileId} not found", certification.SellerProfileId);
                return;
            }

            var user = await _userRepository.GetUserByIdAsync(sellerProfile.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for seller profile {SellerProfileId}", sellerProfile.UserId, certification.SellerProfileId);
                return;
            }

            // Get notification preferences
            var preferences = await _notificationPreferencesRepository.GetByUserIdAsync(user.Id);
            if (preferences == null || !preferences.CertificationStatusNotifications)
            {
                return; // Skip notification if user has disabled this type
            }

            var status = certification.Status;
            var title = status == B2BMarketplace.Core.Enums.CertificationStatus.Approved ? "Certification Approved" : "Certification Rejected";
            var message = status == B2BMarketplace.Core.Enums.CertificationStatus.Approved
                ? $"Your certification '{certification.Name}' has been approved"
                : $"Your certification '{certification.Name}' has been rejected";

            var notificationType = status == B2BMarketplace.Core.Enums.CertificationStatus.Approved
                ? B2BMarketplace.Core.Entities.NotificationType.CertificationApproved
                : B2BMarketplace.Core.Entities.NotificationType.CertificationRejected;

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = notificationType,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                RelatedEntityId = certification.Id,
                RelatedEntityType = "Certification"
            };

            await _notificationRepository.CreateAsync(notification);

            // In a real implementation, this would send an email to the seller
            _logger.LogInformation("Certification status notification created for user {UserId} for certification {CertificationId}",
                user.Id, certification.Id);
        }

        public async Task SendAccountStatusNotificationAsync(User user)
        {
            // Get notification preferences
            var preferences = await _notificationPreferencesRepository.GetByUserIdAsync(user.Id);
            if (preferences == null || !preferences.AccountStatusNotifications)
            {
                return; // Skip notification if user has disabled this type
            }

            var title = user.IsActive ? "Account Unlocked" : "Account Locked";
            var message = user.IsActive ? "Your account has been unlocked" : "Your account has been locked";

            var notificationType = user.IsActive ? B2BMarketplace.Core.Entities.NotificationType.AccountUnlocked : B2BMarketplace.Core.Entities.NotificationType.AccountLocked;

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = notificationType,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                RelatedEntityId = user.Id,
                RelatedEntityType = "User"
            };

            await _notificationRepository.CreateAsync(notification);

            // In a real implementation, this would send an email to the user
            _logger.LogInformation("Account status notification created for user {UserId}", user.Id);
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool? isRead = null, int page = 1, int pageSize = 10)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId, isRead, page, pageSize);

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Type = (B2BMarketplace.Core.DTOs.NotificationType)n.Type,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt,
                RelatedEntityId = n.RelatedEntityId,
                RelatedEntityType = n.RelatedEntityType
            }).ToList();
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null) return null;

            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Type = (B2BMarketplace.Core.DTOs.NotificationType)notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                RelatedEntityId = notification.RelatedEntityId,
                RelatedEntityType = notification.RelatedEntityType
            };
        }

        public async Task MarkNotificationAsReadAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _notificationRepository.UpdateAsync(notification);
            }
        }

        public async Task<int> MarkAllNotificationsAsReadAsync(Guid userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId, isRead: false);
            var readCount = 0;

            foreach (var notification in notifications)
            {
                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    await _notificationRepository.UpdateAsync(notification);
                    readCount++;
                }
            }

            return readCount;
        }

        public async Task<NotificationPreferencesDto> GetUserNotificationPreferencesAsync(Guid userId)
        {
            var preferences = await _notificationPreferencesRepository.GetByUserIdAsync(userId);

            if (preferences == null)
            {
                // Create default preferences if none exist
                preferences = new NotificationPreferences
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    EmailNotificationsEnabled = true,
                    RFQResponseNotifications = true,
                    RFQReceivedNotifications = true,
                    QuoteUpdatedNotifications = true,
                    CertificationStatusNotifications = true,
                    AccountStatusNotifications = true,
                    UpdatedAt = DateTime.UtcNow
                };

                preferences = await _notificationPreferencesRepository.CreateAsync(preferences);
            }

            return new NotificationPreferencesDto
            {
                Id = preferences.Id,
                UserId = preferences.UserId,
                EmailNotificationsEnabled = preferences.EmailNotificationsEnabled,
                RFQResponseNotifications = preferences.RFQResponseNotifications,
                RFQReceivedNotifications = preferences.RFQReceivedNotifications,
                QuoteUpdatedNotifications = preferences.QuoteUpdatedNotifications,
                CertificationStatusNotifications = preferences.CertificationStatusNotifications,
                AccountStatusNotifications = preferences.AccountStatusNotifications,
                UpdatedAt = preferences.UpdatedAt
            };
        }

        public async Task UpdateUserNotificationPreferencesAsync(Guid userId, UpdateNotificationPreferencesDto preferences)
        {
            var existingPreferences = await _notificationPreferencesRepository.GetByUserIdAsync(userId);

            if (existingPreferences == null)
            {
                // Create new preferences
                existingPreferences = new NotificationPreferences
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    EmailNotificationsEnabled = preferences.EmailNotificationsEnabled,
                    RFQResponseNotifications = preferences.RFQResponseNotifications,
                    RFQReceivedNotifications = preferences.RFQReceivedNotifications,
                    QuoteUpdatedNotifications = preferences.QuoteUpdatedNotifications,
                    CertificationStatusNotifications = preferences.CertificationStatusNotifications,
                    AccountStatusNotifications = preferences.AccountStatusNotifications,
                    UpdatedAt = DateTime.UtcNow
                };

                await _notificationPreferencesRepository.CreateAsync(existingPreferences);
            }
            else
            {
                // Update existing preferences
                existingPreferences.EmailNotificationsEnabled = preferences.EmailNotificationsEnabled;
                existingPreferences.RFQResponseNotifications = preferences.RFQResponseNotifications;
                existingPreferences.RFQReceivedNotifications = preferences.RFQReceivedNotifications;
                existingPreferences.QuoteUpdatedNotifications = preferences.QuoteUpdatedNotifications;
                existingPreferences.CertificationStatusNotifications = preferences.CertificationStatusNotifications;
                existingPreferences.AccountStatusNotifications = preferences.AccountStatusNotifications;
                existingPreferences.UpdatedAt = DateTime.UtcNow;

                await _notificationPreferencesRepository.UpdateAsync(existingPreferences);
            }
        }

        public async Task CreateNotificationAsync(Guid userId, B2BMarketplace.Core.DTOs.NotificationType type, string title, string message)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = (B2BMarketplace.Core.Entities.NotificationType)type,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.CreateAsync(notification);
        }
    }
}