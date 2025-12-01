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
    public class QuoteRepository : IQuoteRepository
    {
        private readonly ApplicationDbContext _context;

        public QuoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Quote> CreateAsync(Quote quote)
        {
            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();
            return quote;
        }

        public async Task<Quote?> GetByIdAsync(Guid id)
        {
            return await _context.Quotes
                .Include(q => q.RFQ)
                .Include(q => q.SellerProfile)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Quote>> GetByRFQIdAsync(Guid rfqId)
        {
            return await _context.Quotes
                .Include(q => q.RFQ)
                .Include(q => q.SellerProfile)
                .Where(q => q.RFQId == rfqId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quote>> GetBySellerProfileIdAsync(Guid sellerProfileId)
        {
            return await _context.Quotes
                .Include(q => q.RFQ)
                .Include(q => q.SellerProfile)
                .Where(q => q.SellerProfileId == sellerProfileId)
                .ToListAsync();
        }

        public async Task<Quote> UpdateAsync(Quote quote)
        {
            _context.Quotes.Update(quote);
            await _context.SaveChangesAsync();
            return quote;
        }

        public async Task DeleteAsync(Guid id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote != null)
            {
                _context.Quotes.Remove(quote);
                await _context.SaveChangesAsync();
            }
        }
    }
}