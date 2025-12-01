using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for search operations
    /// </summary>
    public class SearchRepository : ISearchRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for SearchRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public SearchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

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
        public async Task<SearchResultsDto<SellerSearchResultDto>> SearchSellersAsync(
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
            string? sortOrder)
        {
            // Validate page and pageSize
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            // Create the base query joining SellerProfile with User and Certification
            var query = from seller in _context.SellerProfiles
                        join user in _context.Users on seller.UserId equals user.Id
                        where user.Role == Core.Enums.UserRole.Seller
                        select new { seller, user };

            // Apply keyword filter (match against seller profile fields)
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.seller != null && (
                    x.seller.CompanyName != null && x.seller.CompanyName.Contains(keyword) ||
                    x.seller.LegalRepresentative != null && x.seller.LegalRepresentative.Contains(keyword) ||
                    x.seller.TaxId != null && x.seller.TaxId.Contains(keyword) ||
                    x.seller.Industry != null && x.seller.Industry.Contains(keyword) ||
                    x.seller.Country != null && x.seller.Country.Contains(keyword) ||
                    x.seller.Description != null && x.seller.Description.Contains(keyword) ||
                    (x.seller.Products != null && x.seller.Products.Any(p => p != null && ((p.Name != null && p.Name.Contains(keyword)) || (p.Description != null && p.Description.Contains(keyword))))) ||
                    (x.seller.Certifications != null && x.seller.Certifications.Any(c => c != null && c.Name != null && c.Name.Contains(keyword)))
                ));
            }

            // Apply industry filter
            if (!string.IsNullOrEmpty(industry))
            {
                query = query.Where(x => x.seller != null && x.seller.Industry != null && x.seller.Industry.Contains(industry));
            }

            // Apply location filter (country)
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(x => x.seller != null && x.seller.Country.Contains(location));
            }

            // Apply certification filter - need to check if seller has at least one certification matching the filter
            if (!string.IsNullOrEmpty(certification))
            {
                query = query.Where(x => x.seller != null && x.seller.Certifications != null && x.seller.Certifications.Any(c => c != null && c.Name.Contains(certification)));
            }

            // Apply rating range filters
            if (minRating.HasValue)
            {
                query = query.Where(x => x.seller != null && x.seller.AverageRating >= minRating.Value);
            }

            if (maxRating.HasValue)
            {
                query = query.Where(x => x.seller != null && x.seller.AverageRating <= maxRating.Value);
            }

            // Apply premium status filter
            if (isPremium.HasValue)
            {
                query = query.Where(x => x.seller != null && x.seller.IsPremium == isPremium.Value);
            }

            // Apply verified status filter
            if (isVerified.HasValue)
            {
                query = query.Where(x => x.seller != null && x.seller.IsVerified == isVerified.Value);
            }

            // Apply certification types filter
            if (certificationTypes != null && certificationTypes.Any())
            {
                query = query.Where(x => x.seller != null && x.seller.Certifications != null && x.seller.Certifications.Any(c =>
                    c != null && c.Status == Core.Enums.CertificationStatus.Approved &&
                    certificationTypes.Contains(c.Name)));
            }

            // Apply date range filters
            if (createdAfter.HasValue)
            {
                query = query.Where(x => x.seller != null && x.seller.CreatedAt >= createdAfter.Value);
            }

            if (createdBefore.HasValue)
            {
                query = query.Where(x => x.seller != null && x.seller.CreatedAt <= createdBefore.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Determine sort order and apply sorting
            switch (sortBy?.ToLower())
            {
                case "name":
                    query = sortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.seller != null ? x.seller.CompanyName : string.Empty)
                        : query.OrderByDescending(x => x.seller != null ? x.seller.CompanyName : string.Empty);
                    break;
                case "rating":
                    query = sortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.seller != null ? x.seller.AverageRating ?? 0 : 0)
                        : query.OrderByDescending(x => x.seller != null ? x.seller.AverageRating ?? 0 : 0);
                    break;
                case "premium":
                    query = sortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.seller != null ? x.seller.IsPremium : false)
                        : query.OrderByDescending(x => x.seller != null ? x.seller.IsPremium : false);
                    break;
                case "verified":
                    query = sortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.seller != null ? x.seller.IsVerified : false)
                        : query.OrderByDescending(x => x.seller != null ? x.seller.IsVerified : false);
                    break;
                case "created":
                    query = sortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.seller != null ? x.seller.CreatedAt : DateTime.MinValue)
                        : query.OrderByDescending(x => x.seller != null ? x.seller.CreatedAt : DateTime.MinValue);
                    break;
                case "certifications":
                    query = sortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.seller != null && x.seller.Certifications != null ? x.seller.Certifications.Count(c => c != null && c.Status == Core.Enums.CertificationStatus.Approved) : 0)
                        : query.OrderByDescending(x => x.seller != null && x.seller.Certifications != null ? x.seller.Certifications.Count(c => c != null && c.Status == Core.Enums.CertificationStatus.Approved) : 0);
                    break;
                default: // Default to relevance (which we'll implement as most recent first)
                    query = query.OrderByDescending(x => x.seller != null ? x.seller.CreatedAt : DateTime.MinValue);
                    break;
            }

            // Apply pagination
            var sellers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Convert to SellerSearchResultDto
            var results = new List<SellerSearchResultDto>();

            foreach (var item in sellers)
            {
                var certNames = item.seller?.Certifications != null 
                    ? item.seller.Certifications
                        .Where(c => c != null && c.Status == Core.Enums.CertificationStatus.Approved) // Only show approved certifications
                        .Select(c => c != null ? c.Name : string.Empty)
                        .ToList()
                    : new List<string>();

                results.Add(new SellerSearchResultDto
                {
                    Id = item.seller?.Id ?? Guid.Empty,
                    CompanyName = item.seller?.CompanyName ?? string.Empty,
                    LegalRepresentative = item.seller?.LegalRepresentative ?? string.Empty,
                    TaxId = item.seller?.TaxId ?? string.Empty,
                    Industry = item.seller?.Industry ?? string.Empty,
                    Country = item.seller?.Country ?? string.Empty,
                    Description = item.seller?.Description ?? string.Empty,
                    AverageRating = item.seller?.AverageRating,
                    NumberOfRatings = item.seller?.NumberOfRatings ?? 0,
                    IsVerified = item.seller?.IsVerified ?? false,
                    IsPremium = item.seller?.IsPremium ?? false,
                    Certifications = certNames
                });
            }

            // Calculate pagination info
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new SearchResultsDto<SellerSearchResultDto>
            {
                Items = results,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages
            };
        }
    }
}