using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    public interface INotificationPreferencesRepository
    {
        Task<NotificationPreferences?> GetByUserIdAsync(Guid userId);
        Task<NotificationPreferences> CreateAsync(NotificationPreferences preferences);
        Task<NotificationPreferences> UpdateAsync(NotificationPreferences preferences);
    }
}