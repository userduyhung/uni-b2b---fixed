using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BMarketplace.Infrastructure.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductCategory> GetByIdAsync(Guid id)
        {
            var result = await _context.ProductCategories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            return result ?? throw new KeyNotFoundException($"ProductCategory with ID {id} not found");
        }

        public async Task<IEnumerable<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductCategory>> GetActiveAsync()
        {
            return await _context.ProductCategories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<ProductCategory> GetByNameAsync(string name)
        {
            var result = await _context.ProductCategories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
            
            return result ?? throw new KeyNotFoundException($"ProductCategory with name '{name}' not found");
        }

        public async Task<IEnumerable<ProductCategory>> GetByParentIdAsync(Guid parentId)
        {
            return await _context.ProductCategories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .Where(c => c.ParentCategoryId == parentId)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductCategory>> GetRootCategoriesAsync()
        {
            return await _context.ProductCategories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .Where(c => c.ParentCategoryId == null)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductCategory>> FindAsync(System.Linq.Expressions.Expression<Func<ProductCategory, bool>> predicate)
        {
            return await _context.ProductCategories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<ProductCategory> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<ProductCategory, bool>> predicate)
        {
            var result = await _context.ProductCategories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(predicate);
            
            return result ?? throw new KeyNotFoundException("ProductCategory not found for the given predicate");
        }

        public async Task AddAsync(ProductCategory entity)
        {
            await _context.ProductCategories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductCategory entity)
        {
            _context.ProductCategories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category != null)
            {
                _context.ProductCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountAsync()
        {
            return await _context.ProductCategories.CountAsync();
        }

        public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<ProductCategory, bool>> predicate)
        {
            return await _context.ProductCategories.CountAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.ProductCategories.AnyAsync(c => c.Id == id);
        }
    }
}