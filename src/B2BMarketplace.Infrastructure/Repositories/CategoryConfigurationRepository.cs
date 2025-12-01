using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace B2BMarketplace.Infrastructure.Repositories
{
    public class CategoryConfigurationRepository : ICategoryConfigurationRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryConfigurationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryConfiguration> GetByIdAsync(Guid id)
        {
            var result = await _context.CategoryConfigurations
                .Include(cc => cc.Category)
                .FirstOrDefaultAsync(cc => cc.Id == id);
            
            return result ?? throw new KeyNotFoundException($"CategoryConfiguration with ID {id} not found");
        }

        public async Task<IEnumerable<CategoryConfiguration>> GetAllAsync()
        {
            return await _context.CategoryConfigurations
                .Include(cc => cc.Category)
                .ToListAsync();
        }

        public async Task<CategoryConfiguration> GetByCategoryIdAsync(Guid categoryId)
        {
            var result = await _context.CategoryConfigurations
                .Include(cc => cc.Category)
                .FirstOrDefaultAsync(cc => cc.CategoryId == categoryId);
            
            return result ?? throw new KeyNotFoundException($"CategoryConfiguration with CategoryId {categoryId} not found");
        }

        public async Task<bool> ExistsByCategoryIdAsync(Guid categoryId)
        {
            return await _context.CategoryConfigurations.AnyAsync(cc => cc.CategoryId == categoryId);
        }

        public async Task<IEnumerable<CategoryConfiguration>> FindAsync(System.Linq.Expressions.Expression<Func<CategoryConfiguration, bool>> predicate)
        {
            return await _context.CategoryConfigurations
                .Include(cc => cc.Category)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<CategoryConfiguration> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<CategoryConfiguration, bool>> predicate)
        {
            var result = await _context.CategoryConfigurations
                .Include(cc => cc.Category)
                .FirstOrDefaultAsync(predicate);
            
            return result ?? throw new KeyNotFoundException("CategoryConfiguration not found for the given predicate");
        }

        public async Task AddAsync(CategoryConfiguration entity)
        {
            await _context.CategoryConfigurations.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CategoryConfiguration entity)
        {
            _context.CategoryConfigurations.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var configuration = await _context.CategoryConfigurations.FindAsync(id);
            if (configuration != null)
            {
                _context.CategoryConfigurations.Remove(configuration);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountAsync()
        {
            return await _context.CategoryConfigurations.CountAsync();
        }

        public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<CategoryConfiguration, bool>> predicate)
        {
            return await _context.CategoryConfigurations.CountAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.CategoryConfigurations.AnyAsync(cc => cc.Id == id);
        }
    }
}