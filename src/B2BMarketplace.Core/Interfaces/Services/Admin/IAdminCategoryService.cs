using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services.Admin
{
    /// <summary>
    /// Service interface for admin category management
    /// </summary>
    public interface IAdminCategoryService
    {
        /// <summary>
        /// Gets all categories with pagination
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Paginated list of categories</returns>
        Task<PagedResultDto<CategoryDto>> GetCategoriesAsync(int page = 1, int size = 10);

        /// <summary>
        /// Gets a category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category DTO or null if not found</returns>
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id);

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="categoryDto">Category to create</param>
        /// <returns>Created category with ID</returns>
        Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="categoryDto">Updated category data</param>
        /// <returns>Updated category or null if not found</returns>
        Task<CategoryDto?> UpdateCategoryAsync(Guid id, CategoryDto categoryDto);

        /// <summary>
        /// Deletes a category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteCategoryAsync(Guid id);

        /// <summary>
        /// Searches categories by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="page">Page number</param>
        /// <param name="size">Page size</param>
        /// <returns>Paginated list of matching categories</returns>
        Task<PagedResultDto<CategoryDto>> SearchCategoriesAsync(string searchTerm, int page = 1, int size = 10);
    }
}