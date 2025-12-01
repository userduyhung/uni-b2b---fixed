using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for service tier operations
    /// </summary>
    public interface IServiceTierRepository
    {
        /// <summary>
        /// Gets all service tiers
        /// </summary>
        /// <returns>List of service tiers</returns>
        Task<IEnumerable<ServiceTier>> GetServiceTiersAsync();

        /// <summary>
        /// Gets an active service tier by ID
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>Service tier if found, null otherwise</returns>
        Task<ServiceTier?> GetServiceTierByIdAsync(Guid id);

        /// <summary>
        /// Gets a service tier by name
        /// </summary>
        /// <param name="name">Service tier name</param>
        /// <returns>Service tier if found, null otherwise</returns>
        Task<ServiceTier?> GetServiceTierByNameAsync(string name);

        /// <summary>
        /// Gets the default service tier
        /// </summary>
        /// <returns>Default service tier if found, null otherwise</returns>
        Task<ServiceTier?> GetDefaultServiceTierAsync();

        /// <summary>
        /// Adds a new service tier
        /// </summary>
        /// <param name="serviceTier">Service tier to add</param>
        /// <returns>Added service tier</returns>
        Task<ServiceTier> AddServiceTierAsync(ServiceTier serviceTier);

        /// <summary>
        /// Updates an existing service tier
        /// </summary>
        /// <param name="serviceTier">Service tier to update</param>
        /// <returns>Updated service tier</returns>
        Task<ServiceTier> UpdateServiceTierAsync(ServiceTier serviceTier);

        /// <summary>
        /// Deletes a service tier
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        Task<bool> DeleteServiceTierAsync(Guid id);

        /// <summary>
        /// Gets features for a specific service tier
        /// </summary>
        /// <param name="serviceTierId">Service tier ID</param>
        /// <returns>List of features for the service tier</returns>
        Task<IEnumerable<ServiceTierFeature>> GetFeaturesByServiceTierIdAsync(Guid serviceTierId);

        /// <summary>
        /// Adds a feature to a service tier
        /// </summary>
        /// <param name="feature">Feature to add</param>
        /// <returns>Added feature</returns>
        Task<ServiceTierFeature> AddFeatureToServiceTierAsync(ServiceTierFeature feature);

        /// <summary>
        /// Updates a feature in a service tier
        /// </summary>
        /// <param name="feature">Feature to update</param>
        /// <returns>Updated feature</returns>
        Task<ServiceTierFeature> UpdateFeatureInServiceTierAsync(ServiceTierFeature feature);

        /// <summary>
        /// Deletes a feature from a service tier
        /// </summary>
        /// <param name="featureId">Feature ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        Task<bool> DeleteFeatureFromServiceTierAsync(Guid featureId);

        /// <summary>
        /// Gets a service tier feature by ID
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <returns>Service tier feature if found, null otherwise</returns>
        Task<ServiceTierFeature?> GetFeatureByIdAsync(Guid id);

        /// <summary>
        /// Gets configurations for a specific service tier
        /// </summary>
        /// <param name="serviceTierId">Service tier ID</param>
        /// <returns>List of configurations for the service tier</returns>
        Task<IEnumerable<ServiceTierConfiguration>> GetConfigurationsByServiceTierIdAsync(Guid serviceTierId);

        /// <summary>
        /// Adds a configuration to a service tier
        /// </summary>
        /// <param name="configuration">Configuration to add</param>
        /// <returns>Added configuration</returns>
        Task<ServiceTierConfiguration> AddConfigurationToServiceTierAsync(ServiceTierConfiguration configuration);

        /// <summary>
        /// Updates a configuration in a service tier
        /// </summary>
        /// <param name="configuration">Configuration to update</param>
        /// <returns>Updated configuration</returns>
        Task<ServiceTierConfiguration> UpdateConfigurationInServiceTierAsync(ServiceTierConfiguration configuration);

        /// <summary>
        /// Deletes a configuration from a service tier
        /// </summary>
        /// <param name="configurationId">Configuration ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        Task<bool> DeleteConfigurationFromServiceTierAsync(Guid configurationId);

        /// <summary>
        /// Gets a service tier configuration by ID
        /// </summary>
        /// <param name="id">Configuration ID</param>
        /// <returns>Service tier configuration if found, null otherwise</returns>
        Task<ServiceTierConfiguration?> GetConfigurationByIdAsync(Guid id);

        /// <summary>
        /// Gets a service tier configuration by key
        /// </summary>
        /// <param name="serviceTierId">Service tier ID</param>
        /// <param name="key">Configuration key</param>
        /// <returns>Service tier configuration if found, null otherwise</returns>
        Task<ServiceTierConfiguration?> GetConfigurationByKeyAsync(Guid serviceTierId, string key);
    }
}