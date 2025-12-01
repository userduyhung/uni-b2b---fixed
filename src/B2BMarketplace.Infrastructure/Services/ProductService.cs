using B2BMarketplace.Core.DTOs;
using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;

namespace B2BMarketplace.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for Product operations
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ISellerProfileRepository _sellerProfileRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productRepository">Product repository</param>
        /// <param name="sellerProfileRepository">Seller profile repository</param>
        public ProductService(IProductRepository productRepository, ISellerProfileRepository sellerProfileRepository)
        {
            _productRepository = productRepository;
            _sellerProfileRepository = sellerProfileRepository;
        }

        /// <summary>
        /// Get all products with optional filtering
        /// </summary>
        /// <param name="category">Optional category filter</param>
        /// <param name="sellerId">Optional seller ID filter</param>
        /// <returns>Collection of Product DTOs</returns>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(string? category = null, Guid? sellerId = null)
        {
            IEnumerable<Product> products;

            if (category != null && sellerId.HasValue)
            {
                // Filter by both category and seller
                products = (await _productRepository.GetByCategoryAsync(category))
                    .Where(p => p.SellerProfileId == sellerId.Value);
            }
            else if (category != null)
            {
                // Filter by category only
                products = await _productRepository.GetByCategoryAsync(category);
            }
            else if (sellerId.HasValue)
            {
                // Filter by seller only
                products = await _productRepository.GetBySellerIdAsync(sellerId.Value);
            }
            else
            {
                // Get all products
                products = await _productRepository.GetAllAsync();
            }

            return products.Select(MapToDto);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product DTO or null if not found</returns>
        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : MapToDto(product);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="createProductDto">Product creation DTO</param>
        /// <param name="sellerId">Seller user ID</param>
        /// <returns>Created Product DTO</returns>
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, Guid sellerId)
        {
            // Validate that seller profile exists (sellerId is actually the user ID)
            var sellerProfile = await _sellerProfileRepository.GetByUserIdAsync(sellerId);
            if (sellerProfile == null)
            {
                throw new InvalidOperationException("Seller profile not found. Please create a seller profile first before adding products.");
            }

            // Create product entity
            var product = new Product
            {
                SellerProfileId = sellerProfile.Id, // Use the actual seller profile ID
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Category = createProductDto.Category,
                ReferencePrice = createProductDto.ReferencePrice
            };

            // Save to database
            var createdProduct = await _productRepository.CreateAsync(product);

            // Return DTO
            return MapToDto(createdProduct);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="updateProductDto">Product update DTO</param>
        /// <param name="sellerId">Seller user ID</param>
        /// <returns>Updated Product DTO</returns>
        public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto, Guid sellerId)
        {
            // Get seller profile first
            var sellerProfile = await _sellerProfileRepository.GetByUserIdAsync(sellerId);
            if (sellerProfile == null)
            {
                throw new InvalidOperationException("Seller profile not found");
            }

            // Check if product exists and is owned by seller
            var isOwned = await _productRepository.IsOwnedBySellerAsync(id, sellerProfile.Id);
            if (!isOwned)
            {
                throw new UnauthorizedAccessException("Product not found or not owned by seller");
            }

            // Get existing product
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new ArgumentException("Product not found", nameof(id));
            }

            // Update properties if provided
            if (updateProductDto.Name != null)
                product.Name = updateProductDto.Name;

            if (updateProductDto.Description != null)
                product.Description = updateProductDto.Description;

            if (updateProductDto.Category != null)
                product.Category = updateProductDto.Category;

            if (updateProductDto.ReferencePrice.HasValue)
                product.ReferencePrice = updateProductDto.ReferencePrice.Value;

            if (updateProductDto.IsActive.HasValue)
                product.IsActive = updateProductDto.IsActive.Value;

            // Save to database
            var updatedProduct = await _productRepository.UpdateAsync(product);

            // Return DTO
            return MapToDto(updatedProduct);
        }

        /// <summary>
        /// Delete a product (mark as inactive)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="sellerId">Seller user ID</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> DeleteProductAsync(Guid id, Guid sellerId)
        {
            // Get seller profile first
            var sellerProfile = await _sellerProfileRepository.GetByUserIdAsync(sellerId);
            if (sellerProfile == null)
            {
                return false;
            }

            // Check if product exists and is owned by seller
            var isOwned = await _productRepository.IsOwnedBySellerAsync(id, sellerProfile.Id);
            if (!isOwned)
            {
                return false;
            }

            // Delete product
            return await _productRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Update product inventory
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">New inventory quantity</param>
        /// <param name="sellerId">Seller ID</param>
        /// <returns>Updated Product DTO</returns>
        public async Task<ProductDto> UpdateProductInventoryAsync(Guid id, int quantity, Guid sellerId)
        {
            // Get seller profile first
            var sellerProfile = await _sellerProfileRepository.GetByUserIdAsync(sellerId);
            if (sellerProfile == null)
            {
                throw new InvalidOperationException("Seller profile not found");
            }

            // Check if product exists and is owned by seller
            var isOwned = await _productRepository.IsOwnedBySellerAsync(id, sellerProfile.Id);
            if (!isOwned)
            {
                throw new UnauthorizedAccessException("Product not found or not owned by seller");
            }

            // Update stock quantity using the repository method
            var updatedProduct = await _productRepository.UpdateStockQuantityAsync(id, quantity);
            if (updatedProduct == null)
            {
                throw new ArgumentException("Product not found", nameof(id));
            }

            // Return DTO
            return MapToDto(updatedProduct);
        }

        /// <summary>
        /// Map Product entity to Product DTO
        /// </summary>
        /// <param name="product">Product entity</param>
        /// <returns>Product DTO</returns>
        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                SellerProfileId = product.SellerProfileId,
                Name = product.Name,
                Description = product.Description,
                ImagePath = product.ImagePath,
                Category = product.Category,
                ReferencePrice = product.ReferencePrice,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                IsActive = product.IsActive,
                StockQuantity = product.StockQuantity
            };
        }
    }
}