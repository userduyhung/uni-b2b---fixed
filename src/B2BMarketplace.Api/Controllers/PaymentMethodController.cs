using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2BMarketplace.Core.Interfaces.Services;
using B2BMarketplace.Core.Entities;
using System;
using System.Threading.Tasks;

namespace B2BMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/payment/methods")]
    public class MethodsController : BaseController
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public MethodsController(IPaymentMethodService paymentMethodService, ITokenService tokenService) : base(tokenService)
        {
            _paymentMethodService = paymentMethodService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] CreatePaymentMethodDto dto)
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var paymentMethod = new PaymentMethod
                {
                    Type = dto.Type ?? string.Empty,
                    CardNumber = dto.CardNumber ?? string.Empty,
                    ExpiryDate = dto.ExpiryDate ?? string.Empty,
                    CVV = dto.CVV ?? string.Empty, // In a real app, this should be handled more securely
                    CardholderName = dto.CardholderName ?? string.Empty,
                    IsDefault = dto.IsDefault
                };

                var result = await _paymentMethodService.CreatePaymentMethodAsync(userId, paymentMethod);
                return Ok(new { success = true, data = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPaymentMethods()
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var paymentMethods = await _paymentMethodService.GetPaymentMethodsByUserIdAsync(userId);
                return Ok(new { success = true, data = paymentMethods });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentMethod(string id)
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var paymentMethod = await _paymentMethodService.GetPaymentMethodByIdAsync(id, userId);
                
                if (paymentMethod == null)
                {
                    return NotFound(new { success = false, message = "Payment method not found" });
                }

                return Ok(new { success = true, data = paymentMethod });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePaymentMethod(string id, [FromBody] UpdatePaymentMethodDto dto)
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var paymentMethod = new PaymentMethod
                {
                    Type = dto.Type ?? string.Empty,
                    CardNumber = dto.CardNumber ?? string.Empty,
                    ExpiryDate = dto.ExpiryDate ?? string.Empty,
                    CVV = dto.CVV ?? string.Empty,
                    CardholderName = dto.CardholderName ?? string.Empty,
                    IsDefault = dto.IsDefault
                };

                var result = await _paymentMethodService.UpdatePaymentMethodAsync(id, userId, paymentMethod);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Payment method not found" });
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
        public async Task<IActionResult> DeletePaymentMethod(string id)
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var result = await _paymentMethodService.DeletePaymentMethodAsync(id, userId);
                
                if (!result)
                {
                    return NotFound(new { success = false, message = "Payment method not found" });
                }

                return Ok(new { success = true, message = "Payment method deleted successfully" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}/default")]
        [Authorize]
        public async Task<IActionResult> SetDefaultPaymentMethod(string id)
        {
            try
            {
                var userIdString = GetUserIdFromClaims();
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest(new { success = false, message = "Invalid user ID format" });
                }
                
                var result = await _paymentMethodService.SetDefaultPaymentMethodAsync(userId, id);
                
                if (result == null)
                {
                    return NotFound(new { success = false, message = "Payment method not found" });
                }

                return Ok(new { success = true, data = result });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CreatePaymentMethodDto
    {
        public string? Type { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public string? CardholderName { get; set; }
        public bool IsDefault { get; set; } = false;
    }

    public class UpdatePaymentMethodDto
    {
        public string? Type { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public string? CardholderName { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}
