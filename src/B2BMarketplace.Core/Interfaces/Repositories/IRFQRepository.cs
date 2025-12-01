using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    public interface IRFQRepository
    {
        Task<RFQ?> GetByIdAsync(Guid id);
        Task<IEnumerable<RFQ>> GetByBuyerProfileIdAsync(Guid buyerProfileId);
        Task<IEnumerable<RFQ>> GetBySellerProfileIdAsync(Guid sellerProfileId);
        Task<RFQ> CreateAsync(RFQ rfq);
        Task<RFQ> UpdateAsync(RFQ rfq);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<RFQ>> GetAllAsync();

        /// <summary>
        /// Gets the total count of RFQs
        /// </summary>
        /// <returns>Total number of RFQs</returns>
        Task<int> GetTotalCountAsync();

        /// <summary>
        /// Gets RFQs created in the specified date range
        /// </summary>
        /// <param name="startDate">Start date for the range</param>
        /// <param name="endDate">End date for the range</param>
        /// <returns>List of RFQs created in the date range</returns>
        Task<IEnumerable<RFQ>> GetRFQsInDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}