using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Product entity operations
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Database context</param>
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>Collection of Product entities</returns>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Get products by seller ID
        /// </summary>
        /// <param name="sellerId">Seller profile ID</param>
        /// <returns>Collection of Product entities</returns>
        public async Task<IEnumerable<Product>> GetBySellerIdAsync(Guid sellerId)
        {
            return await _context.Products
                .Where(p => p.SellerProfileId == sellerId && p.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        /// <param name="category">Category to filter by</param>
        /// <returns>Collection of Product entities</returns>
        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.Category == category && p.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product entity or null if not found</returns>
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="product">Product entity to create</param>
        /// <returns>Created Product entity</returns>
        public async Task<Product> CreateAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            product.IsActive = true;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="product">Product entity with updated values</param>
        /// <returns>Updated Product entity</returns>
        public async Task<Product> UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return product;
        }

        /// <summary>
        /// Delete a product (mark as inactive)
        /// </summary>
        /// <param name="id">Product ID to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Check if product exists
        /// </summary>
        /// <param name="id">Product ID to check</param>
        /// <returns>True if product exists, false otherwise</returns>
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Products
                .AnyAsync(p => p.Id == id && p.IsActive);
        }

        /// <summary>
        /// Check if product is owned by seller
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="sellerId">Seller ID</param>
        /// <returns>True if product is owned by seller, false otherwise</returns>
        public async Task<bool> IsOwnedBySellerAsync(Guid productId, Guid sellerId)
        {
            return await _context.Products
                .AnyAsync(p => p.Id == productId && p.SellerProfileId == sellerId && p.IsActive);
        }

        /// <summary>
        /// Gets the total count of products
        /// </summary>
        /// <returns>Total number of products</returns>
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Products.CountAsync(p => p.IsActive);
        }

        /// <summary>
        /// Get products by seller profile ID
        /// </summary>
        /// <param name="sellerProfileId">Seller profile ID</param>
        /// <returns>Collection of Product entities</returns>
        public async Task<IEnumerable<Product>> GetBySellerProfileIdAsync(Guid sellerProfileId)
        {
            return await _context.Products
                .Where(p => p.SellerProfileId == sellerProfileId && p.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Update the stock quantity of a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">New stock quantity</param>
        /// <returns>Updated Product entity or null if product not found</returns>
        public async Task<Product?> UpdateStockQuantityAsync(Guid id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return null;

            product.StockQuantity = quantity;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return product;
        }
    }
}