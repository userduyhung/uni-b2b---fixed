using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface IQuoteService
    {
        Task<QuoteDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<QuoteDto>> GetByRFQIdAsync(Guid rfqId);
        Task<IEnumerable<QuoteDto>> GetBySellerProfileIdAsync(Guid sellerProfileId);
        Task<QuoteDto> CreateAsync(Guid rfqId, CreateQuoteDto createQuoteDto, Guid sellerProfileId);
        Task<QuoteDto> UpdateAsync(Guid id, CreateQuoteDto updateQuoteDto);
        Task DeleteAsync(Guid id);
    }
}