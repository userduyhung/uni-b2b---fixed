using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface for search operations
    /// </summary>
    public interface ISearchRepository
    {
        /// <summary>
        /// Searches for sellers based on the provided criteria
        /// </summary>
        /// <param name="keyword">Search keyword to match against seller names, descriptions, products</param>
        /// <param name="industry">Industry to filter by</param>
        /// <param name="location">Location to filter by</param>
        /// <param name="certification">Certification to filter by</param>
        /// <param name="minRating">Minimum rating filter (0-5)</param>
        /// <param name="maxRating">Maximum rating filter (0-5)</param>
        /// <param name="isPremium">Filter by premium status</param>
        /// <param name="isVerified">Filter by verified status</param>
        /// <param name="certificationTypes">List of certification types to filter by</param>
        /// <param name="createdAfter">Date range start for seller profile creation</param>
        /// <param name="createdBefore">Date range end for seller profile creation</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <param name="sortBy">Sort by field</param>
        /// <param name="sortOrder">Sort order</param>
        /// <returns>Paginated search results</returns>
        Task<SearchResultsDto<SellerSearchResultDto>> SearchSellersAsync(
            string? keyword,
            string? industry,
            string? location,
            string? certification,
            double? minRating,
            double? maxRating,
            bool? isPremium,
            bool? isVerified,
            List<string>? certificationTypes,
            DateTime? createdAfter,
            DateTime? createdBefore,
            int page,
            int pageSize,
            string? sortBy,
            string? sortOrder);
    }
}