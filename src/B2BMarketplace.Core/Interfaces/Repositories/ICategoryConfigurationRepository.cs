using B2BMarketplace.Core.Entities;
using System;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    public interface ICategoryConfigurationRepository : IGenericRepository<CategoryConfiguration>
    {
        Task<CategoryConfiguration> GetByCategoryIdAsync(Guid categoryId);
        Task<bool> ExistsByCategoryIdAsync(Guid categoryId);
    }
}