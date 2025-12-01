using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for search operations
    /// </summary>
    public class SearchService : ISearchService
    {
        private readonly ISearchRepository _searchRepository;

        /// <summary>
        /// Constructor for SearchService
        /// </summary>
        /// <param name="searchRepository">Search repository</param>
        public SearchService(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        /// <summary>
        /// Performs an advanced search for sellers based on the provided criteria
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Paginated search results</returns>
        public async Task<SearchResultsDto<SellerSearchResultDto>> SearchSellersAsync(SearchRequestDto request)
        {
            // Validate request
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Validate page and pageSize
            if (request.Page < 1) request.Page = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 50) request.PageSize = 50;

            // Validate rating parameters
            if (request.MinRating.HasValue && (request.MinRating < 0 || request.MinRating > 5))
            {
                request.MinRating = null; // Reset invalid value
            }

            if (request.MaxRating.HasValue && (request.MaxRating < 0 || request.MaxRating > 5))
            {
                request.MaxRating = null; // Reset invalid value
            }

            // Ensure min rating is not greater than max rating
            if (request.MinRating.HasValue && request.MaxRating.HasValue && request.MinRating > request.MaxRating)
            {
                request.MinRating = null; // Reset invalid range
                request.MaxRating = null;
            }

            // Validate sorting parameters
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var validSortFields = new[] { "name", "rating", "premium", "verified", "relevance", "created", "certifications" };
                if (!validSortFields.Contains(request.SortBy.ToLower()))
                {
                    request.SortBy = "relevance"; // Default to relevance if invalid
                }
            }

            if (!string.IsNullOrEmpty(request.SortOrder))
            {
                var validSortOrders = new[] { "asc", "desc" };
                if (!validSortOrders.Contains(request.SortOrder.ToLower()))
                {
                    request.SortOrder = "desc"; // Default to desc if invalid
                }
            }

            // Use Query if provided, otherwise use Keyword
            var searchTerm = !string.IsNullOrEmpty(request.Query) ? request.Query : request.Keyword;

            // Perform the search
            return await _searchRepository.SearchSellersAsync(
                searchTerm,
                request.Industry,
                request.Location,
                request.Certification,
                request.MinRating,
                request.MaxRating,
                request.IsPremium,
                request.IsVerified,
                request.CertificationTypes,
                request.CreatedAfter,
                request.CreatedBefore,
                request.Page,
                request.PageSize,
                request.SortBy,
                request.SortOrder);
        }

        /// <summary>
        /// Performs a search for products based on the provided criteria
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Paginated search results</returns>
        public async Task<SearchResultsDto<ProductDto>> SearchProductsAsync(SearchRequestDto request)
        {
            // Validate request
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Validate page and pageSize
            if (request.Page < 1) request.Page = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 50) request.PageSize = 50;

            // Use Query if provided, otherwise use Keyword
            var searchTerm = !string.IsNullOrEmpty(request.Query) ? request.Query : request.Keyword;

            // For now, return empty results as we don't have product search implemented
            return await Task.FromResult(new SearchResultsDto<ProductDto>
            {
                Items = new List<ProductDto>(),
                TotalCount = 0,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = 0,
                HasPreviousPage = false,
                HasNextPage = false
            });
        }
    }
}