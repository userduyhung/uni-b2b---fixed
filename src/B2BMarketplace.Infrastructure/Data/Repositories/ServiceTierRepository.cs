using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for service tier data access operations
    /// </summary>
    public class ServiceTierRepository : IServiceTierRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor for ServiceTierRepository
        /// </summary>
        /// <param name="context">Database context</param>
        public ServiceTierRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all service tiers
        /// </summary>
        /// <returns>List of service tiers</returns>
        public async Task<IEnumerable<ServiceTier>> GetServiceTiersAsync()
        {
            return await _context.ServiceTiers
                .Include(st => st.Features)
                .ToListAsync();
        }

        /// <summary>
        /// Gets an active service tier by ID
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>Service tier if found, null otherwise</returns>
        public async Task<ServiceTier?> GetServiceTierByIdAsync(Guid id)
        {
            return await _context.ServiceTiers
                .Include(st => st.Features)
                .FirstOrDefaultAsync(st => st.Id == id && st.IsActive);
        }

        /// <summary>
        /// Gets a service tier by name
        /// </summary>
        /// <param name="name">Service tier name</param>
        /// <returns>Service tier if found, null otherwise</returns>
        public async Task<ServiceTier?> GetServiceTierByNameAsync(string name)
        {
            return await _context.ServiceTiers
                .Include(st => st.Features)
                .FirstOrDefaultAsync(st => st.Name == name);
        }

        /// <summary>
        /// Gets the default service tier
        /// </summary>
        /// <returns>Default service tier if found, null otherwise</returns>
        public async Task<ServiceTier?> GetDefaultServiceTierAsync()
        {
            return await _context.ServiceTiers
                .Include(st => st.Features)
                .FirstOrDefaultAsync(st => st.IsDefault);
        }

        /// <summary>
        /// Adds a new service tier
        /// </summary>
        /// <param name="serviceTier">Service tier to add</param>
        /// <returns>Added service tier</returns>
        public async Task<ServiceTier> AddServiceTierAsync(ServiceTier serviceTier)
        {
            _context.ServiceTiers.Add(serviceTier);
            await _context.SaveChangesAsync();
            return serviceTier;
        }

        /// <summary>
        /// Updates an existing service tier
        /// </summary>
        /// <param name="serviceTier">Service tier to update</param>
        /// <returns>Updated service tier</returns>
        public async Task<ServiceTier> UpdateServiceTierAsync(ServiceTier serviceTier)
        {
            serviceTier.UpdatedAt = DateTime.UtcNow;
            _context.ServiceTiers.Update(serviceTier);
            await _context.SaveChangesAsync();
            return serviceTier;
        }

        /// <summary>
        /// Deletes a service tier
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public async Task<bool> DeleteServiceTierAsync(Guid id)
        {
            var serviceTier = await _context.ServiceTiers.FindAsync(id);
            if (serviceTier == null)
                return false;

            _context.ServiceTiers.Remove(serviceTier);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets features for a specific service tier
        /// </summary>
        /// <param name="serviceTierId">Service tier ID</param>
        /// <returns>List of features for the service tier</returns>
        public async Task<IEnumerable<ServiceTierFeature>> GetFeaturesByServiceTierIdAsync(Guid serviceTierId)
        {
            return await _context.ServiceTierFeatures
                .Where(stf => stf.ServiceTierId == serviceTierId)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a feature to a service tier
        /// </summary>
        /// <param name="feature">Feature to add</param>
        /// <returns>Added feature</returns>
        public async Task<ServiceTierFeature> AddFeatureToServiceTierAsync(ServiceTierFeature feature)
        {
            _context.ServiceTierFeatures.Add(feature);
            await _context.SaveChangesAsync();
            return feature;
        }

        /// <summary>
        /// Updates a feature in a service tier
        /// </summary>
        /// <param name="feature">Feature to update</param>
        /// <returns>Updated feature</returns>
        public async Task<ServiceTierFeature> UpdateFeatureInServiceTierAsync(ServiceTierFeature feature)
        {
            feature.UpdatedAt = DateTime.UtcNow;
            _context.ServiceTierFeatures.Update(feature);
            await _context.SaveChangesAsync();
            return feature;
        }

        /// <summary>
        /// Deletes a feature from a service tier
        /// </summary>
        /// <param name="featureId">Feature ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public async Task<bool> DeleteFeatureFromServiceTierAsync(Guid featureId)
        {
            var feature = await _context.ServiceTierFeatures.FindAsync(featureId);
            if (feature == null)
                return false;

            _context.ServiceTierFeatures.Remove(feature);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a service tier feature by ID
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <returns>Service tier feature if found, null otherwise</returns>
        public async Task<ServiceTierFeature?> GetFeatureByIdAsync(Guid id)
        {
            return await _context.ServiceTierFeatures.FindAsync(id);
        }

        /// <summary>
        /// Gets configurations for a specific service tier
        /// </summary>
        /// <param name="serviceTierId">Service tier ID</param>
        /// <returns>List of configurations for the service tier</returns>
        public async Task<IEnumerable<ServiceTierConfiguration>> GetConfigurationsByServiceTierIdAsync(Guid serviceTierId)
        {
            return await _context.ServiceTierConfigurations
                .Where(stc => stc.ServiceTierId == serviceTierId)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a configuration to a service tier
        /// </summary>
        /// <param name="configuration">Configuration to add</param>
        /// <returns>Added configuration</returns>
        public async Task<ServiceTierConfiguration> AddConfigurationToServiceTierAsync(ServiceTierConfiguration configuration)
        {
            _context.ServiceTierConfigurations.Add(configuration);
            await _context.SaveChangesAsync();
            return configuration;
        }

        /// <summary>
        /// Updates a configuration in a service tier
        /// </summary>
        /// <param name="configuration">Configuration to update</param>
        /// <returns>Updated configuration</returns>
        public async Task<ServiceTierConfiguration> UpdateConfigurationInServiceTierAsync(ServiceTierConfiguration configuration)
        {
            configuration.UpdatedAt = DateTime.UtcNow;
            _context.ServiceTierConfigurations.Update(configuration);
            await _context.SaveChangesAsync();
            return configuration;
        }

        /// <summary>
        /// Deletes a configuration from a service tier
        /// </summary>
        /// <param name="configurationId">Configuration ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public async Task<bool> DeleteConfigurationFromServiceTierAsync(Guid configurationId)
        {
            var configuration = await _context.ServiceTierConfigurations.FindAsync(configurationId);
            if (configuration == null)
                return false;

            _context.ServiceTierConfigurations.Remove(configuration);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets a service tier configuration by ID
        /// </summary>
        /// <param name="id">Configuration ID</param>
        /// <returns>Service tier configuration if found, null otherwise</returns>
        public async Task<ServiceTierConfiguration?> GetConfigurationByIdAsync(Guid id)
        {
            return await _context.ServiceTierConfigurations.FindAsync(id);
        }

        /// <summary>
        /// Gets a service tier configuration by key
        /// </summary>
        /// <param name="serviceTierId">Service tier ID</param>
        /// <param name="key">Configuration key</param>
        /// <returns>Service tier configuration if found, null otherwise</returns>
        public async Task<ServiceTierConfiguration?> GetConfigurationByKeyAsync(Guid serviceTierId, string key)
        {
            return await _context.ServiceTierConfigurations
                .FirstOrDefaultAsync(stc => stc.ServiceTierId == serviceTierId && stc.Key == key);
        }
    }
}