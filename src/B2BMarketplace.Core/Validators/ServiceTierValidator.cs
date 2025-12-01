using B2BMarketplace.Core.DTOs;
using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.Validators
{
    /// <summary>
    /// Validator for service tier DTOs
    /// </summary>
    public static class ServiceTierValidator
    {
        /// <summary>
        /// Validates a CreateUpdateServiceTierDto
        /// </summary>
        /// <param name="dto">The DTO to validate</param>
        /// <returns>Validation results</returns>
        public static List<ValidationResult> ValidateCreateUpdateServiceTierDto(CreateUpdateServiceTierDto dto)
        {
            var validationResults = new List<ValidationResult>();

            // Validate Name
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                validationResults.Add(new ValidationResult("Name is required", new[] { nameof(dto.Name) }));
            }
            else if (dto.Name.Length > 100)
            {
                validationResults.Add(new ValidationResult("Name must be 100 characters or less", new[] { nameof(dto.Name) }));
            }

            // Validate Description
            if (dto.Description != null && dto.Description.Length > 500)
            {
                validationResults.Add(new ValidationResult("Description must be 500 characters or less", new[] { nameof(dto.Description) }));
            }

            // Validate Price
            if (dto.Price < 0)
            {
                validationResults.Add(new ValidationResult("Price must be zero or positive", new[] { nameof(dto.Price) }));
            }

            // Validate Features
            if (dto.Features != null)
            {
                for (int i = 0; i < dto.Features.Count; i++)
                {
                    var feature = dto.Features[i];
                    if (string.IsNullOrWhiteSpace(feature.Name))
                    {
                        validationResults.Add(new ValidationResult($"Feature {i + 1}: Name is required", new[] { $"{nameof(dto.Features)}[{i}].{nameof(feature.Name)}" }));
                    }
                    else if (feature.Name.Length > 100)
                    {
                        validationResults.Add(new ValidationResult($"Feature {i + 1}: Name must be 100 characters or less", new[] { $"{nameof(dto.Features)}[{i}].{nameof(feature.Name)}" }));
                    }

                    if (feature.Description != null && feature.Description.Length > 500)
                    {
                        validationResults.Add(new ValidationResult($"Feature {i + 1}: Description must be 500 characters or less", new[] { $"{nameof(dto.Features)}[{i}].{nameof(feature.Description)}" }));
                    }
                }
            }

            return validationResults;
        }

        /// <summary>
        /// Validates a CreateUpdateServiceTierFeatureDto
        /// </summary>
        /// <param name="dto">The DTO to validate</param>
        /// <returns>Validation results</returns>
        public static List<ValidationResult> ValidateCreateUpdateServiceTierFeatureDto(CreateUpdateServiceTierFeatureDto dto)
        {
            var validationResults = new List<ValidationResult>();

            // Validate Name
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                validationResults.Add(new ValidationResult("Name is required", new[] { nameof(dto.Name) }));
            }
            else if (dto.Name.Length > 100)
            {
                validationResults.Add(new ValidationResult("Name must be 100 characters or less", new[] { nameof(dto.Name) }));
            }

            // Validate Description
            if (dto.Description != null && dto.Description.Length > 500)
            {
                validationResults.Add(new ValidationResult("Description must be 500 characters or less", new[] { nameof(dto.Description) }));
            }

            return validationResults;
        }
    }
}