using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Interface for service tier service operations
    /// </summary>
    public interface IServiceTierService
    {
        /// <summary>
        /// Gets all service tiers
        /// </summary>
        /// <returns>List of service tier DTOs</returns>
        Task<IEnumerable<ServiceTierDto>> GetServiceTiersAsync();

        /// <summary>
        /// Gets a service tier by ID
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>Service tier DTO if found, null otherwise</returns>
        Task<ServiceTierDto?> GetServiceTierByIdAsync(Guid id);

        /// <summary>
        /// Gets a service tier by name
        /// </summary>
        /// <param name="name">Service tier name</param>
        /// <returns>Service tier DTO if found, null otherwise</returns>
        Task<ServiceTierDto?> GetServiceTierByNameAsync(string name);

        /// <summary>
        /// Gets the default service tier
        /// </summary>
        /// <returns>Default service tier DTO if found, null otherwise</returns>
        Task<ServiceTierDto?> GetDefaultServiceTierAsync();

        /// <summary>
        /// Creates a new service tier
        /// </summary>
        /// <param name="createServiceTierDto">Service tier creation parameters</param>
        /// <returns>Created service tier DTO</returns>
        Task<ServiceTierDto> CreateServiceTierAsync(CreateUpdateServiceTierDto createServiceTierDto);

        /// <summary>
        /// Updates an existing service tier
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <param name="updateServiceTierDto">Service tier update parameters</param>
        /// <returns>Updated service tier DTO</returns>
        Task<ServiceTierDto> UpdateServiceTierAsync(Guid id, CreateUpdateServiceTierDto updateServiceTierDto);

        /// <summary>
        /// Deletes a service tier
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        Task<bool> DeleteServiceTierAsync(Guid id);

        /// <summary>
        /// Adds a feature to a service tier
        /// </summary>
        /// <param name="createFeatureDto">Feature creation parameters</param>
        /// <returns>Added feature DTO</returns>
        Task<ServiceTierFeatureDto> AddFeatureToServiceTierAsync(CreateUpdateServiceTierFeatureDto createFeatureDto);

        /// <summary>
        /// Updates a feature in a service tier
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <param name="updateFeatureDto">Feature update parameters</param>
        /// <returns>Updated feature DTO</returns>
        Task<ServiceTierFeatureDto> UpdateFeatureInServiceTierAsync(Guid id, CreateUpdateServiceTierFeatureDto updateFeatureDto);

        /// <summary>
        /// Deletes a feature from a service tier
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        Task<bool> DeleteFeatureFromServiceTierAsync(Guid id);

        /// <summary>
        /// Gets a service tier feature by ID
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <returns>Service tier feature DTO if found, null otherwise</returns>
        Task<ServiceTierFeatureDto?> GetFeatureByIdAsync(Guid id);

        /// <summary>
        /// Gets features for a specific service tier
        /// </summary>
        /// <param name="serviceTierId">Service tier ID</param>
        /// <returns>List of service tier feature DTOs</returns>
        Task<IEnumerable<ServiceTierFeatureDto>> GetFeaturesByServiceTierIdAsync(Guid serviceTierId);
    }
}