using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// DTO for advanced search queries
    /// </summary>
    public class SearchRequestDto
    {
        /// <summary>
        /// Simple search query
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Search keyword to match against seller names, descriptions, products
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Industry to filter by
        /// </summary>
        public string? Industry { get; set; }

        /// <summary>
        /// Location to filter by (Country, City, etc.)
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Certification to filter by
        /// </summary>
        public string? Certification { get; set; }

        /// <summary>
        /// Minimum rating filter (0-5)
        /// </summary>
        public double? MinRating { get; set; }

        /// <summary>
        /// Maximum rating filter (0-5)
        /// </summary>
        public double? MaxRating { get; set; }

        /// <summary>
        /// Filter by premium status
        /// </summary>
        public bool? IsPremium { get; set; }

        /// <summary>
        /// Filter by verified status
        /// </summary>
        public bool? IsVerified { get; set; }

        /// <summary>
        /// List of certification types to filter by
        /// </summary>
        public List<string>? CertificationTypes { get; set; }

        /// <summary>
        /// Date range start for seller profile creation
        /// </summary>
        public DateTime? CreatedAfter { get; set; }

        /// <summary>
        /// Date range end for seller profile creation
        /// </summary>
        public DateTime? CreatedBefore { get; set; }

        /// <summary>
        /// Page number for pagination (default: 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of results per page (default: 10, max: 50)
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Sort by field (relevance, rating, name, etc.)
        /// </summary>
        public string? SortBy { get; set; } = "relevance";

        /// <summary>
        /// Sort order (asc, desc)
        /// </summary>
        public string? SortOrder { get; set; } = "desc";
    }

    /// <summary>
    /// DTO for search results
    /// </summary>
    public class SearchResultsDto<T>
    {
        /// <summary>
        /// List of items matching the search criteria
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Total number of items matching the search criteria (before pagination)
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Page number of the results
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNextPage { get; set; }
    }

    /// <summary>
    /// DTO for a seller search result item
    /// </summary>
    public class SellerSearchResultDto
    {
        /// <summary>
        /// Seller's ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Seller's company name
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// Seller's legal representative
        /// </summary>
        public string LegalRepresentative { get; set; } = string.Empty;

        /// <summary>
        /// Tax ID
        /// </summary>
        public string TaxId { get; set; } = string.Empty;

        /// <summary>
        /// Industry
        /// </summary>
        public string Industry { get; set; } = string.Empty;

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Seller's description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Average rating
        /// </summary>
        public double? AverageRating { get; set; }

        /// <summary>
        /// Number of ratings received
        /// </summary>
        public int NumberOfRatings { get; set; }

        /// <summary>
        /// Whether the seller is verified
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Whether the seller is premium
        /// </summary>
        public bool IsPremium { get; set; }

        /// <summary>
        /// List of certifications
        /// </summary>
        public IEnumerable<string> Certifications { get; set; } = new List<string>();
    }
}