using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    public interface IQuoteRepository
    {
        Task<Quote?> GetByIdAsync(Guid id);
        Task<IEnumerable<Quote>> GetByRFQIdAsync(Guid rfqId);
        Task<IEnumerable<Quote>> GetBySellerProfileIdAsync(Guid sellerProfileId);
        Task<Quote> CreateAsync(Quote quote);
        Task<Quote> UpdateAsync(Quote quote);
        Task DeleteAsync(Guid id);
    }
}