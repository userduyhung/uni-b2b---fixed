using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    public class RFQRepository : IRFQRepository
    {
        private readonly ApplicationDbContext _context;

        public RFQRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RFQ> CreateAsync(RFQ rfq)
        {
            _context.RFQs.Add(rfq);
            await _context.SaveChangesAsync();
            return rfq;
        }

        public async Task<RFQ?> GetByIdAsync(Guid id)
        {
            return await _context.RFQs
                .Include(r => r.BuyerProfile)
                .Include(r => r.Items)
                .Include(r => r.Recipients)
                    .ThenInclude(rr => rr.SellerProfile)
                .Include(r => r.Quotes)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<RFQ>> GetByBuyerProfileIdAsync(Guid buyerProfileId)
        {
            return await _context.RFQs
                .Include(r => r.BuyerProfile)
                .Include(r => r.Items)
                .Include(r => r.Recipients)
                    .ThenInclude(rr => rr.SellerProfile)
                .Include(r => r.Quotes)
                .Where(r => r.BuyerProfileId == buyerProfileId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RFQ>> GetBySellerProfileIdAsync(Guid sellerProfileId)
        {
            return await _context.RFQs
                .Include(r => r.BuyerProfile)
                .Include(r => r.Items)
                .Include(r => r.Recipients)
                    .ThenInclude(rr => rr.SellerProfile)
                .Include(r => r.Quotes)
                .Where(r => r.Recipients.Any(rr => rr.SellerProfileId == sellerProfileId))
                .ToListAsync();
        }

        public async Task<RFQ> UpdateAsync(RFQ rfq)
        {
            _context.RFQs.Update(rfq);
            await _context.SaveChangesAsync();
            return rfq;
        }

        public async Task DeleteAsync(Guid id)
        {
            var rfq = await _context.RFQs.FindAsync(id);
            if (rfq != null)
            {
                _context.RFQs.Remove(rfq);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<RFQ>> GetAllAsync()
        {
            return await _context.RFQs
                .Include(r => r.BuyerProfile)
                .Include(r => r.Items)
                .Include(r => r.Recipients)
                    .ThenInclude(rr => rr.SellerProfile)
                .Include(r => r.Quotes)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the total count of RFQs
        /// </summary>
        /// <returns>Total number of RFQs</returns>
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.RFQs.CountAsync();
        }

        /// <summary>
        /// Gets RFQs created in the specified date range
        /// </summary>
        /// <param name="startDate">Start date for the range</param>
        /// <param name="endDate">End date for the range</param>
        /// <returns>List of RFQs created in the date range</returns>
        public async Task<IEnumerable<RFQ>> GetRFQsInDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.RFQs
                .Include(r => r.BuyerProfile)
                .Include(r => r.Items)
                .Include(r => r.Recipients)
                    .ThenInclude(rr => rr.SellerProfile)
                .Include(r => r.Quotes)
                .Where(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate)
                .ToListAsync();
        }
    }
}