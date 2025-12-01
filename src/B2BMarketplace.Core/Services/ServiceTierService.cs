using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using System.Linq;

namespace B2BMarketplace.Core.Services
{
    /// <summary>
    /// Service for service tier operations
    /// </summary>
    public class ServiceTierService : IServiceTierService
    {
        private readonly IServiceTierRepository _serviceTierRepository;

        /// <summary>
        /// Constructor for ServiceTierService
        /// </summary>
        /// <param name="serviceTierRepository">Service tier repository</param>
        public ServiceTierService(IServiceTierRepository serviceTierRepository)
        {
            _serviceTierRepository = serviceTierRepository;
        }

        /// <summary>
        /// Gets all service tiers
        /// </summary>
        /// <returns>List of service tier DTOs</returns>
        public async Task<IEnumerable<ServiceTierDto>> GetServiceTiersAsync()
        {
            var serviceTiers = await _serviceTierRepository.GetServiceTiersAsync();

            return serviceTiers.Select(tier => new ServiceTierDto
            {
                Id = tier.Id,
                Name = tier.Name,
                Description = tier.Description,
                Price = tier.Price,
                IsDefault = tier.IsDefault,
                IsActive = tier.IsActive,
                CreatedAt = tier.CreatedAt,
                UpdatedAt = tier.UpdatedAt,
                Features = tier.Features.Select(f => new ServiceTierFeatureDto
                {
                    Id = f.Id,
                    ServiceTierId = f.ServiceTierId,
                    Name = f.Name,
                    Description = f.Description,
                    IsAvailable = f.IsAvailable,
                    Value = f.Value,
                    DisplayOrder = f.DisplayOrder,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList()
            });
        }

        /// <summary>
        /// Gets a service tier by ID
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>Service tier DTO if found, null otherwise</returns>
        public async Task<ServiceTierDto?> GetServiceTierByIdAsync(Guid id)
        {
            var serviceTier = await _serviceTierRepository.GetServiceTierByIdAsync(id);
            if (serviceTier == null)
            {
                return null;
            }

            return new ServiceTierDto
            {
                Id = serviceTier.Id,
                Name = serviceTier.Name,
                Description = serviceTier.Description,
                Price = serviceTier.Price,
                IsDefault = serviceTier.IsDefault,
                IsActive = serviceTier.IsActive,
                CreatedAt = serviceTier.CreatedAt,
                UpdatedAt = serviceTier.UpdatedAt,
                Features = serviceTier.Features.Select(f => new ServiceTierFeatureDto
                {
                    Id = f.Id,
                    ServiceTierId = f.ServiceTierId,
                    Name = f.Name,
                    Description = f.Description,
                    IsAvailable = f.IsAvailable,
                    Value = f.Value,
                    DisplayOrder = f.DisplayOrder,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList()
            };
        }

        /// <summary>
        /// Gets a service tier by name
        /// </summary>
        /// <param name="name">Service tier name</param>
        /// <returns>Service tier DTO if found, null otherwise</returns>
        public async Task<ServiceTierDto?> GetServiceTierByNameAsync(string name)
        {
            var serviceTier = await _serviceTierRepository.GetServiceTierByNameAsync(name);
            if (serviceTier == null)
            {
                return null;
            }

            return new ServiceTierDto
            {
                Id = serviceTier.Id,
                Name = serviceTier.Name,
                Description = serviceTier.Description,
                Price = serviceTier.Price,
                IsDefault = serviceTier.IsDefault,
                IsActive = serviceTier.IsActive,
                CreatedAt = serviceTier.CreatedAt,
                UpdatedAt = serviceTier.UpdatedAt,
                Features = serviceTier.Features.Select(f => new ServiceTierFeatureDto
                {
                    Id = f.Id,
                    ServiceTierId = f.ServiceTierId,
                    Name = f.Name,
                    Description = f.Description,
                    IsAvailable = f.IsAvailable,
                    Value = f.Value,
                    DisplayOrder = f.DisplayOrder,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList()
            };
        }

        /// <summary>
        /// Gets the default service tier
        /// </summary>
        /// <returns>Default service tier DTO if found, null otherwise</returns>
        public async Task<ServiceTierDto?> GetDefaultServiceTierAsync()
        {
            var serviceTier = await _serviceTierRepository.GetDefaultServiceTierAsync();
            if (serviceTier == null)
            {
                return null;
            }

            return new ServiceTierDto
            {
                Id = serviceTier.Id,
                Name = serviceTier.Name,
                Description = serviceTier.Description,
                Price = serviceTier.Price,
                IsDefault = serviceTier.IsDefault,
                IsActive = serviceTier.IsActive,
                CreatedAt = serviceTier.CreatedAt,
                UpdatedAt = serviceTier.UpdatedAt,
                Features = serviceTier.Features.Select(f => new ServiceTierFeatureDto
                {
                    Id = f.Id,
                    ServiceTierId = f.ServiceTierId,
                    Name = f.Name,
                    Description = f.Description,
                    IsAvailable = f.IsAvailable,
                    Value = f.Value,
                    DisplayOrder = f.DisplayOrder,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList()
            };
        }

        /// <summary>
        /// Creates a new service tier
        /// </summary>
        /// <param name="createServiceTierDto">Service tier creation parameters</param>
        /// <returns>Created service tier DTO</returns>
        public async Task<ServiceTierDto> CreateServiceTierAsync(CreateUpdateServiceTierDto createServiceTierDto)
        {
            // Check if a default tier already exists and if we're setting this as default
            if (createServiceTierDto.IsDefault)
            {
                var existingDefault = await _serviceTierRepository.GetDefaultServiceTierAsync();
                if (existingDefault != null)
                {
                    // Unset the existing default tier
                    existingDefault.IsDefault = false;
                    existingDefault.UpdatedAt = DateTime.UtcNow;
                    await _serviceTierRepository.UpdateServiceTierAsync(existingDefault);
                }
            }

            var serviceTier = new ServiceTier
            {
                Name = createServiceTierDto.Name,
                Description = createServiceTierDto.Description,
                Price = createServiceTierDto.Price,
                IsDefault = createServiceTierDto.IsDefault,
                IsActive = createServiceTierDto.IsActive,
                UpdatedAt = DateTime.UtcNow
            };

            var addedServiceTier = await _serviceTierRepository.AddServiceTierAsync(serviceTier);

            // Add features
            foreach (var feature in createServiceTierDto.Features)
            {
                var serviceTierFeature = new ServiceTierFeature
                {
                    ServiceTierId = addedServiceTier.Id,
                    Name = feature.Name,
                    Description = feature.Description,
                    IsAvailable = feature.IsAvailable,
                    Value = feature.Value,
                    DisplayOrder = feature.DisplayOrder,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _serviceTierRepository.AddFeatureToServiceTierAsync(serviceTierFeature);
            }

            // Refresh the service tier with features to return
            var createdServiceTier = await _serviceTierRepository.GetServiceTierByIdAsync(addedServiceTier.Id);
            
            if (createdServiceTier == null)
            {
                throw new InvalidOperationException("Failed to retrieve the created service tier");
            }

            return new ServiceTierDto
            {
                Id = createdServiceTier.Id,
                Name = createdServiceTier.Name,
                Description = createdServiceTier.Description,
                Price = createdServiceTier.Price,
                IsDefault = createdServiceTier.IsDefault,
                IsActive = createdServiceTier.IsActive,
                CreatedAt = createdServiceTier.CreatedAt,
                UpdatedAt = createdServiceTier.UpdatedAt,
                Features = createdServiceTier.Features.Select(f => new ServiceTierFeatureDto
                {
                    Id = f.Id,
                    ServiceTierId = f.ServiceTierId,
                    Name = f.Name,
                    Description = f.Description,
                    IsAvailable = f.IsAvailable,
                    Value = f.Value,
                    DisplayOrder = f.DisplayOrder,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList()
            };
        }

        /// <summary>
        /// Updates an existing service tier
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <param name="updateServiceTierDto">Service tier update parameters</param>
        /// <returns>Updated service tier DTO</returns>
        public async Task<ServiceTierDto> UpdateServiceTierAsync(Guid id, CreateUpdateServiceTierDto updateServiceTierDto)
        {
            var existingServiceTier = await _serviceTierRepository.GetServiceTierByIdAsync(id);
            if (existingServiceTier == null)
            {
                throw new ArgumentException($"Service tier with ID {id} not found");
            }

            // Check if we're setting this as default
            if (updateServiceTierDto.IsDefault)
            {
                var existingDefault = await _serviceTierRepository.GetDefaultServiceTierAsync();
                if (existingDefault != null && existingDefault.Id != id)
                {
                    // Unset the existing default tier
                    existingDefault.IsDefault = false;
                    existingDefault.UpdatedAt = DateTime.UtcNow;
                    await _serviceTierRepository.UpdateServiceTierAsync(existingDefault);
                }
            }

            existingServiceTier.Name = updateServiceTierDto.Name;
            existingServiceTier.Description = updateServiceTierDto.Description;
            existingServiceTier.Price = updateServiceTierDto.Price;
            existingServiceTier.IsDefault = updateServiceTierDto.IsDefault;
            existingServiceTier.IsActive = updateServiceTierDto.IsActive;
            existingServiceTier.UpdatedAt = DateTime.UtcNow;

            var updatedServiceTier = await _serviceTierRepository.UpdateServiceTierAsync(existingServiceTier);

            // Update features
            // First, get existing features for this tier to update or delete
            var existingFeatures = await _serviceTierRepository.GetFeaturesByServiceTierIdAsync(id);

            // Create a lookup for the new features
            var newFeatureIds = updateServiceTierDto.Features
                .Where(f => f.Id != Guid.Empty) // Only consider existing features
                .Select(f => f.Id)
                .ToHashSet();

            // Update existing features or add new ones
            foreach (var featureDto in updateServiceTierDto.Features)
            {
                var existingFeature = existingFeatures.FirstOrDefault(f => f.Id == featureDto.Id);

                if (existingFeature != null)
                {
                    // Update existing feature
                    existingFeature.Name = featureDto.Name;
                    existingFeature.Description = featureDto.Description;
                    existingFeature.IsAvailable = featureDto.IsAvailable;
                    existingFeature.Value = featureDto.Value;
                    existingFeature.DisplayOrder = featureDto.DisplayOrder;
                    existingFeature.UpdatedAt = DateTime.UtcNow;

                    await _serviceTierRepository.UpdateFeatureInServiceTierAsync(existingFeature);
                }
                else
                {
                    // Add new feature
                    var newFeature = new ServiceTierFeature
                    {
                        ServiceTierId = id,
                        Name = featureDto.Name,
                        Description = featureDto.Description,
                        IsAvailable = featureDto.IsAvailable,
                        Value = featureDto.Value,
                        DisplayOrder = featureDto.DisplayOrder,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _serviceTierRepository.AddFeatureToServiceTierAsync(newFeature);
                }
            }

            // Remove features that are no longer in the update (not in the new list)
            foreach (var existingFeature in existingFeatures)
            {
                if (!newFeatureIds.Contains(existingFeature.Id))
                {
                    await _serviceTierRepository.DeleteFeatureFromServiceTierAsync(existingFeature.Id);
                }
            }

            // Refresh the service tier with features to return
            var refreshedServiceTier = await _serviceTierRepository.GetServiceTierByIdAsync(id);
            
            if (refreshedServiceTier == null)
            {
                throw new InvalidOperationException("Failed to retrieve the updated service tier");
            }

            return new ServiceTierDto
            {
                Id = refreshedServiceTier.Id,
                Name = refreshedServiceTier.Name,
                Description = refreshedServiceTier.Description,
                Price = refreshedServiceTier.Price,
                IsDefault = refreshedServiceTier.IsDefault,
                IsActive = refreshedServiceTier.IsActive,
                CreatedAt = refreshedServiceTier.CreatedAt,
                UpdatedAt = refreshedServiceTier.UpdatedAt,
                Features = refreshedServiceTier.Features.Select(f => new ServiceTierFeatureDto
                {
                    Id = f.Id,
                    ServiceTierId = f.ServiceTierId,
                    Name = f.Name,
                    Description = f.Description,
                    IsAvailable = f.IsAvailable,
                    Value = f.Value,
                    DisplayOrder = f.DisplayOrder,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList()
            };
        }

        /// <summary>
        /// Deletes a service tier
        /// </summary>
        /// <param name="id">Service tier ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public async Task<bool> DeleteServiceTierAsync(Guid id)
        {
            // Check if the service tier exists
            var serviceTier = await _serviceTierRepository.GetServiceTierByIdAsync(id);
            if (serviceTier == null)
            {
                return false;
            }

            // Don't allow deletion of the default tier if there are other tiers
            var allTiers = await _serviceTierRepository.GetServiceTiersAsync();
            if (serviceTier.IsDefault && allTiers.Count() > 1)
            {
                throw new InvalidOperationException("Cannot delete the default service tier when other tiers exist. Please set another tier as default first.");
            }

            return await _serviceTierRepository.DeleteServiceTierAsync(id);
        }

        /// <summary>
        /// Adds a feature to a service tier
        /// </summary>
        /// <param name="createFeatureDto">Feature creation parameters</param>
        /// <returns>Added feature DTO</returns>
        public async Task<ServiceTierFeatureDto> AddFeatureToServiceTierAsync(CreateUpdateServiceTierFeatureDto createFeatureDto)
        {
            var feature = new ServiceTierFeature
            {
                ServiceTierId = createFeatureDto.ServiceTierId,
                Name = createFeatureDto.Name,
                Description = createFeatureDto.Description,
                IsAvailable = createFeatureDto.IsAvailable,
                Value = createFeatureDto.Value,
                DisplayOrder = createFeatureDto.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var addedFeature = await _serviceTierRepository.AddFeatureToServiceTierAsync(feature);

            return new ServiceTierFeatureDto
            {
                Id = addedFeature.Id,
                ServiceTierId = addedFeature.ServiceTierId,
                Name = addedFeature.Name,
                Description = addedFeature.Description,
                IsAvailable = addedFeature.IsAvailable,
                Value = addedFeature.Value,
                DisplayOrder = addedFeature.DisplayOrder,
                CreatedAt = addedFeature.CreatedAt,
                UpdatedAt = addedFeature.UpdatedAt
            };
        }

        /// <summary>
        /// Updates a feature in a service tier
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <param name="updateFeatureDto">Feature update parameters</param>
        /// <returns>Updated feature DTO</returns>
        public async Task<ServiceTierFeatureDto> UpdateFeatureInServiceTierAsync(Guid id, CreateUpdateServiceTierFeatureDto updateFeatureDto)
        {
            var existingFeature = await _serviceTierRepository.GetFeaturesByServiceTierIdAsync(updateFeatureDto.ServiceTierId)
                .ContinueWith(task => task.Result.FirstOrDefault(f => f.Id == id));

            if (existingFeature == null)
            {
                throw new ArgumentException($"Feature with ID {id} not found in service tier {updateFeatureDto.ServiceTierId}");
            }

            existingFeature.Name = updateFeatureDto.Name;
            existingFeature.Description = updateFeatureDto.Description;
            existingFeature.IsAvailable = updateFeatureDto.IsAvailable;
            existingFeature.Value = updateFeatureDto.Value;
            existingFeature.DisplayOrder = updateFeatureDto.DisplayOrder;
            existingFeature.UpdatedAt = DateTime.UtcNow;

            var updatedFeature = await _serviceTierRepository.UpdateFeatureInServiceTierAsync(existingFeature);

            return new ServiceTierFeatureDto
            {
                Id = updatedFeature.Id,
                ServiceTierId = updatedFeature.ServiceTierId,
                Name = updatedFeature.Name,
                Description = updatedFeature.Description,
                IsAvailable = updatedFeature.IsAvailable,
                Value = updatedFeature.Value,
                DisplayOrder = updatedFeature.DisplayOrder,
                CreatedAt = updatedFeature.CreatedAt,
                UpdatedAt = updatedFeature.UpdatedAt
            };
        }

        /// <summary>
        /// Deletes a feature from a service tier
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public async Task<bool> DeleteFeatureFromServiceTierAsync(Guid id)
        {
            return await _serviceTierRepository.DeleteFeatureFromServiceTierAsync(id);
        }

        /// <summary>
        /// Gets a service tier feature by ID
        /// </summary>
        /// <param name="id">Feature ID</param>
        /// <returns>Service tier feature DTO if found, null otherwise</returns>
        public async Task<ServiceTierFeatureDto?> GetFeatureByIdAsync(Guid id)
        {
            var feature = await _serviceTierRepository.GetFeatureByIdAsync(id);
            if (feature == null)
            {
                return null;
            }

            return new ServiceTierFeatureDto
            {
                Id = feature.Id,
                ServiceTierId = feature.ServiceTierId,
                Name = feature.Name,
                Description = feature.Description,
                IsAvailable = feature.IsAvailable,
                Value = feature.Value,
                DisplayOrder = feature.DisplayOrder,
                CreatedAt = feature.CreatedAt,
                UpdatedAt = feature.UpdatedAt
            };
        }

        /// <summary>
        /// Gets features for a specific service tier
        /// </summary>
        /// <param name="serviceTierId">Service tier ID</param>
        /// <returns>List of service tier feature DTOs</returns>
        public async Task<IEnumerable<ServiceTierFeatureDto>> GetFeaturesByServiceTierIdAsync(Guid serviceTierId)
        {
            var features = await _serviceTierRepository.GetFeaturesByServiceTierIdAsync(serviceTierId);

            return features.Select(feature => new ServiceTierFeatureDto
            {
                Id = feature.Id,
                ServiceTierId = feature.ServiceTierId,
                Name = feature.Name,
                Description = feature.Description,
                IsAvailable = feature.IsAvailable,
                Value = feature.Value,
                DisplayOrder = feature.DisplayOrder,
                CreatedAt = feature.CreatedAt,
                UpdatedAt = feature.UpdatedAt
            });
        }
    }
}