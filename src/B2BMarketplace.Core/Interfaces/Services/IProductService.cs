using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for Product operations
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Get all products with optional filtering
        /// </summary>
        /// <param name="category">Optional category filter</param>
        /// <param name="sellerId">Optional seller ID filter</param>
        /// <returns>Collection of Product DTOs</returns>
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(string? category = null, Guid? sellerId = null);

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product DTO or null if not found</returns>
        Task<ProductDto?> GetProductByIdAsync(Guid id);

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="createProductDto">Product creation DTO</param>
        /// <param name="sellerId">Seller ID</param>
        /// <returns>Created Product DTO</returns>
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, Guid sellerId);

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Product update DTO</param>
        /// <param name="sellerId">Seller ID</param>
        /// <returns>Updated Product DTO</returns>
        Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto, Guid sellerId);

        /// <summary>
        /// Delete a product (mark as inactive)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="sellerId">Seller ID</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteProductAsync(Guid id, Guid sellerId);

        /// <summary>
        /// Update product inventory
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">New inventory quantity</param>
        /// <param name="sellerId">Seller ID</param>
        /// <returns>Updated Product DTO</returns>
        Task<ProductDto> UpdateProductInventoryAsync(Guid id, int quantity, Guid sellerId);
    }
}