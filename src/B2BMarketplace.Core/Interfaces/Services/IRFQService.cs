using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface IRFQService
    {
        Task<IEnumerable<RFQDto>> GetAllAsync();
        Task<RFQDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<RFQDto>> GetByBuyerProfileIdAsync(Guid buyerProfileId);
        Task<IEnumerable<RFQDto>> GetBySellerProfileIdAsync(Guid sellerProfileId);
        Task<RFQDto> CreateAsync(CreateRFQDto createRFQDto, Guid buyerProfileId);
        Task<RFQDto> UpdateStatusAsync(Guid id, UpdateRFQStatusDto updateRFQStatusDto);
        Task<RFQDto> CloseRFQAsync(Guid id, Guid buyerProfileId);
        Task DeleteAsync(Guid id);
    }
}