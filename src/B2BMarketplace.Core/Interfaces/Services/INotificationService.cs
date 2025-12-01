using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface INotificationService
    {
        // Existing methods
        Task SendQuoteSubmittedNotificationAsync(Quote quote);
        Task SendRFQResponseNotificationAsync(RFQ rfq, Quote quote);

        // New notification methods
        Task SendRFQReceivedNotificationAsync(RFQ rfq, Guid sellerProfileId);
        Task SendQuoteUpdatedNotificationAsync(Quote quote);
        Task SendCertificationStatusNotificationAsync(Certification certification);
        Task SendAccountStatusNotificationAsync(User user);

        // Notification management methods
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool? isRead = null, int page = 1, int pageSize = 10);
        Task<NotificationDto?> GetNotificationByIdAsync(Guid id);
        Task MarkNotificationAsReadAsync(Guid id);
        Task<int> MarkAllNotificationsAsReadAsync(Guid userId);

        // Notification preferences methods
        Task<NotificationPreferencesDto> GetUserNotificationPreferencesAsync(Guid userId);
        Task UpdateUserNotificationPreferencesAsync(Guid userId, UpdateNotificationPreferencesDto preferences);

        // Generic notification creation
        Task CreateNotificationAsync(Guid userId, DTOs.NotificationType type, string title, string message);
    }
}