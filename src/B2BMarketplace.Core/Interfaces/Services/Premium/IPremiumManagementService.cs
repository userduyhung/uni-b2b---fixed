using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services.Premium
{
    /// <summary>
    /// Service interface for premium management operations
    /// </summary>
    public interface IPremiumManagementService
    {
        /// <summary>
        /// Gets all premium service tiers with their features
        /// </summary>
        /// <returns>List of service tier DTOs</returns>
        Task<IEnumerable<ServiceTierDto>> GetServiceTiersAsync();

        /// <summary>
        /// Gets a premium service tier by ID
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>Service tier DTO or null if not found</returns>
        Task<ServiceTierDto?> GetServiceTierByIdAsync(Guid id);

        /// <summary>
        /// Creates a new premium service tier
        /// </summary>
        /// <param name="serviceTierDto">Service tier to create</param>
        /// <returns>Created service tier with ID</returns>
        Task<ServiceTierDto> CreateServiceTierAsync(ServiceTierDto serviceTierDto);

        /// <summary>
        /// Updates an existing service tier
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <param name="serviceTierDto">Updated service tier data</param>
        /// <returns>Updated service tier or null if not found</returns>
        Task<ServiceTierDto?> UpdateServiceTierAsync(Guid id, ServiceTierDto serviceTierDto);

        /// <summary>
        /// Deletes a service tier by ID
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteServiceTierAsync(Guid id);

        /// <summary>
        /// Gets premium subscription history for a seller
        /// </summary>
        /// <param name="sellerId">ID of the seller</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Paginated list of subscription history</returns>
        Task<PagedResultDto<SubscriptionHistoryDto>> GetSubscriptionHistoryAsync(Guid sellerId, int page = 1, int size = 10);

        /// <summary>
        /// Gets premium seller analytics
        /// </summary>
        /// <param name="startDate">Start date for analytics</param>
        /// <param name="endDate">End date for analytics</param>
        /// <returns>Premium seller analytics data</returns>
        Task<PremiumAnalyticsDto> GetPremiumAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Processes premium renewal for a seller
        /// </summary>
        /// <param name="sellerId">ID of the seller</param>
        /// <param name="newTierId">ID of the new service tier</param>
        /// <returns>True if renewal was successful, false otherwise</returns>
        Task<bool> ProcessRenewalAsync(Guid sellerId, Guid newTierId);
    }
}