using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    public class RFQRecipientRepository : IRFQRecipientRepository
    {
        private readonly ApplicationDbContext _context;

        public RFQRecipientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RFQRecipient?> GetByIdAsync(Guid id)
        {
            return await _context.RFQRecipients.FindAsync(id);
        }

        public async Task<IEnumerable<RFQRecipient>> GetByRFQIdAsync(Guid rfqId)
        {
            return await _context.RFQRecipients
                .Where(r => r.RFQId == rfqId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RFQRecipient>> GetBySellerProfileIdAsync(Guid sellerProfileId)
        {
            return await _context.RFQRecipients
                .Where(r => r.SellerProfileId == sellerProfileId)
                .ToListAsync();
        }

        public async Task<RFQRecipient> CreateAsync(RFQRecipient recipient)
        {
            _context.RFQRecipients.Add(recipient);
            await _context.SaveChangesAsync();
            return recipient;
        }

        public async Task<RFQRecipient> UpdateAsync(RFQRecipient recipient)
        {
            _context.RFQRecipients.Update(recipient);
            await _context.SaveChangesAsync();
            return recipient;
        }

        public async Task DeleteAsync(Guid id)
        {
            var recipient = await _context.RFQRecipients.FindAsync(id);
            if (recipient != null)
            {
                _context.RFQRecipients.Remove(recipient);
                await _context.SaveChangesAsync();
            }
        }
    }
}