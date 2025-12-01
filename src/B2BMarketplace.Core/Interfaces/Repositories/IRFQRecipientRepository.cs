using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    public interface IRFQRecipientRepository
    {
        Task<RFQRecipient?> GetByIdAsync(Guid id);
        Task<IEnumerable<RFQRecipient>> GetByRFQIdAsync(Guid rfqId);
        Task<IEnumerable<RFQRecipient>> GetBySellerProfileIdAsync(Guid sellerProfileId);
        Task<RFQRecipient> CreateAsync(RFQRecipient recipient);
        Task<RFQRecipient> UpdateAsync(RFQRecipient recipient);
        Task DeleteAsync(Guid id);
    }
}