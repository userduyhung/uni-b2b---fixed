using B2BMarketplace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    public interface IProductCategoryRepository : IGenericRepository<ProductCategory>
    {
        Task<IEnumerable<ProductCategory>> GetActiveAsync();
        Task<ProductCategory> GetByNameAsync(string name);
        Task<IEnumerable<ProductCategory>> GetByParentIdAsync(Guid parentId);
        Task<IEnumerable<ProductCategory>> GetRootCategoriesAsync();
    }
}