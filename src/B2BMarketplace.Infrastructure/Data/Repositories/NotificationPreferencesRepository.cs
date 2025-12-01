using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    public class NotificationPreferencesRepository : INotificationPreferencesRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationPreferencesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationPreferences?> GetByUserIdAsync(Guid userId)
        {
            return await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId);
        }

        public async Task<NotificationPreferences> CreateAsync(NotificationPreferences preferences)
        {
            _context.NotificationPreferences.Add(preferences);
            await _context.SaveChangesAsync();
            return preferences;
        }

        public async Task<NotificationPreferences> UpdateAsync(NotificationPreferences preferences)
        {
            _context.NotificationPreferences.Update(preferences);
            await _context.SaveChangesAsync();
            return preferences;
        }
    }
}