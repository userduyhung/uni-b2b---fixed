using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Entities;
using System;
using System.Threading.Tasks;

namespace B2BMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/profile/addresses")]
    public class AddressesController : BaseController
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService, ITokenService tokenService) : base(tokenService)
        {
            _addressService = addressService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto dto)
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var address = new Address
                {
                    RecipientName = dto.RecipientName ?? string.Empty,
                    Street = dto.Street ?? string.Empty,
                    City = dto.City ?? string.Empty,
                    State = dto.State ?? string.Empty,
                    ZipCode = dto.ZipCode ?? string.Empty,
                    Country = dto.Country ?? string.Empty,
                    IsDefault = dto.IsDefault
                };

                var result = await _addressService.CreateAddressAsync(userId, address);
                return Ok(new { success = true, data = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAddresses()
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
                return Ok(new { success = true, data = addresses });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAddress(string id)
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var address = await _addressService.GetAddressByIdAsync(id, userId);
                
                if (address == null)
                {
                    return NotFound(new { success = false, message = "Address not found" });
                }

                return Ok(new { success = true, data = address });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAddress(string id, [FromBody] UpdateAddressDto dto)
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var address = new Address
                {
                    RecipientName = dto.RecipientName ?? string.Empty,
                    Street = dto.Street ?? string.Empty,
                    City = dto.City ?? string.Empty,
                    State = dto.State ?? string.Empty,
                    ZipCode = dto.ZipCode ?? string.Empty,
                    Country = dto.Country ?? string.Empty,
                    IsDefault = dto.IsDefault
                };

                var result = await _addressService.UpdateAddressAsync(id, userId, address);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Address not found" });
                }

                return Ok(new { success = true, data = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAddress(string id)
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var result = await _addressService.DeleteAddressAsync(id, userId);
                
                if (!result)
                {
                    return NotFound(new { success = false, message = "Address not found" });
                }

                return Ok(new { success = true, message = "Address deleted successfully" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}/default")]
        [Authorize]
        public async Task<IActionResult> SetDefaultAddress(string id)
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var result = await _addressService.SetDefaultAddressAsync(userId, id);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Address not found" });
                }

                return Ok(new { success = true, data = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CreateAddressDto
    {
        public string? RecipientName { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public bool IsDefault { get; set; } = false;
    }

    public class UpdateAddressDto
    {
        public string? RecipientName { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}
