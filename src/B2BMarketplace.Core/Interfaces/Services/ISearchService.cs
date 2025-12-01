using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for search operations
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Performs an advanced search for sellers based on the provided criteria
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Paginated search results</returns>
        Task<SearchResultsDto<SellerSearchResultDto>> SearchSellersAsync(SearchRequestDto request);

        /// <summary>
        /// Performs a search for products based on the provided criteria
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Paginated search results</returns>
        Task<SearchResultsDto<ProductDto>> SearchProductsAsync(SearchRequestDto request);
    }
}