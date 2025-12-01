using B2BMarketplace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface IAddressService
    {
        Task<Address> CreateAddressAsync(Guid userId, Address address);
        Task<Address> GetAddressByIdAsync(string addressId, Guid userId);
        Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId);
        Task<Address> UpdateAddressAsync(string addressId, Guid userId, Address address);
        Task<bool> DeleteAddressAsync(string addressId, Guid userId);
        Task<Address> SetDefaultAddressAsync(Guid userId, string addressId);
    }
}