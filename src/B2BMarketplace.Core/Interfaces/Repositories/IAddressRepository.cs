using B2BMarketplace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    public interface IAddressRepository
    {
        Task<Address> CreateAddressAsync(Address address);
        Task<Address> GetAddressByIdAsync(string addressId);
        Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId);
        Task<Address> UpdateAddressAsync(Address address);
        Task<bool> DeleteAddressAsync(string addressId);
        Task<Address> SetDefaultAddressAsync(Guid userId, string addressId);
        Task<bool> SaveChangesAsync();
    }
}