using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<Address> CreateAddressAsync(Guid userId, Address address)
        {
            // Set the user ID to the one provided, not from the input
            address.UserId = userId;
            address.Id = Guid.NewGuid().ToString();
            address.CreatedAt = DateTime.UtcNow;
            address.UpdatedAt = DateTime.UtcNow;

            // If this is the first address for the user, set it as default
            var existingAddresses = await _addressRepository.GetAddressesByUserIdAsync(userId);
            // Convert to list to check count
            var addressList = existingAddresses.ToList();
            if (addressList.Count == 0)
            {
                address.IsDefault = true;
            }

            return await _addressRepository.CreateAddressAsync(address);
        }

        public async Task<Address> GetAddressByIdAsync(string addressId, Guid userId)
        {
            var address = await _addressRepository.GetAddressByIdAsync(addressId);
            
            // Verify that the address belongs to the user
            if (address == null || address.UserId != userId)
            {
                return null;
            }

            return address;
        }

        public async Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId)
        {
            return await _addressRepository.GetAddressesByUserIdAsync(userId);
        }

        public async Task<Address> UpdateAddressAsync(string addressId, Guid userId, Address address)
        {
            var existingAddress = await _addressRepository.GetAddressByIdAsync(addressId);
            
            // Verify that the address belongs to the user
            if (existingAddress == null || existingAddress.UserId != userId)
            {
                return null;
            }

            // Update the address properties
            existingAddress.RecipientName = address.RecipientName;
            existingAddress.Street = address.Street;
            existingAddress.City = address.City;
            existingAddress.State = address.State;
            existingAddress.ZipCode = address.ZipCode;
            existingAddress.Country = address.Country;
            existingAddress.UpdatedAt = DateTime.UtcNow;

            // Only update the default status if explicitly provided in the update
            if (address.IsDefault)
            {
                // If changing this to default, set all other addresses for this user as non-default
                await _addressRepository.SetDefaultAddressAsync(userId, addressId);
            }
            else
            {
                // If not setting as default, just update the record
                existingAddress.IsDefault = address.IsDefault;
            }

            return await _addressRepository.UpdateAddressAsync(existingAddress);
        }

        public async Task<bool> DeleteAddressAsync(string addressId, Guid userId)
        {
            var existingAddress = await _addressRepository.GetAddressByIdAsync(addressId);
            
            // Verify that the address belongs to the user
            if (existingAddress == null || existingAddress.UserId != userId)
            {
                return false;
            }

            return await _addressRepository.DeleteAddressAsync(addressId);
        }

        public async Task<Address> SetDefaultAddressAsync(Guid userId, string addressId)
        {
            return await _addressRepository.SetDefaultAddressAsync(userId, addressId);
        }
    }
}