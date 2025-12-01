using B2BMarketplace.Core.Entities;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for Product entity operations
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>Collection of Product entities</returns>
        Task<IEnumerable<Product>> GetAllAsync();

        /// <summary>
        /// Get products by seller ID
        /// </summary>
        /// <param name="sellerId">Seller profile ID</param>
        /// <returns>Collection of Product entities</returns>
        Task<IEnumerable<Product>> GetBySellerIdAsync(Guid sellerId);

        /// <summary>
        /// Get products by seller profile ID
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Collection of Product entities</returns>
        Task<IEnumerable<Product>> GetBySellerProfileIdAsync(Guid sellerProfileId);

        /// <summary>
        /// Get products by category
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <returns>Collection of Product entities</returns>
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product entity or null if not found</returns>
        Task<Product?> GetByIdAsync(Guid id);

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="product">Product entity to create</param>
        /// <returns>Created Product entity</returns>
        Task<Product> CreateAsync(Product product);

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="product">Product entity with updated values</param>
        /// <returns>Updated Product entity</returns>
        Task<Product> UpdateAsync(Product product);

        /// <summary>
        /// Delete a product (mark as inactive)
        /// </summary>
        /// <param name="id">Product ID to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Check if product exists
        /// </summary>
        /// <param name="id">Product ID to check</param>
        /// <returns>True if product exists, false otherwise</returns>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Check if product is owned by seller
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="sellerId">Seller ID</param>
        /// <returns>True if product is owned by seller, false otherwise</returns>
        Task<bool> IsOwnedBySellerAsync(Guid productId, Guid sellerId);

        /// <summary>
        /// Gets the total count of products
        /// </summary>
        /// <returns>Total number of products</returns>
        Task<int> GetTotalCountAsync();

        /// <summary>
        /// Update the stock quantity of a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">New stock quantity</param>
        /// <returns>Updated Product entity or null if product not found</returns>
        Task<Product?> UpdateStockQuantityAsync(Guid id, int quantity);
    }
}