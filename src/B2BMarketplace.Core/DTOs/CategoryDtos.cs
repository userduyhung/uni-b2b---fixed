using System;

namespace B2BMarketplace.Core.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CategoryConfigurationDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? RequiredCertifications { get; set; } // JSON string
        public string? AdditionalFields { get; set; } // JSON string
        public bool AllowsVerifiedBadge { get; set; }
        public int MinCertificationsForBadge { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateCategoryConfigurationDto
    {
        public Guid CategoryId { get; set; }
        public string? RequiredCertifications { get; set; } // JSON string
        public string? AdditionalFields { get; set; } // JSON string
        public bool AllowsVerifiedBadge { get; set; }
        public int MinCertificationsForBadge { get; set; } = 0;
    }

    public class UpdateCategoryConfigurationDto
    {
        public string? RequiredCertifications { get; set; } // JSON string
        public string? AdditionalFields { get; set; } // JSON string
        public bool AllowsVerifiedBadge { get; set; }
        public int MinCertificationsForBadge { get; set; } = 0;
    }
}