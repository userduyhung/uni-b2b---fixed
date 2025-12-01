using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<PaymentMethod> CreatePaymentMethodAsync(Guid userId, PaymentMethod paymentMethod)
        {
            // Set the user ID to the one provided, not from the input
            paymentMethod.UserId = userId;
            paymentMethod.Id = Guid.NewGuid().ToString();
            paymentMethod.IsActive = true;
            paymentMethod.CreatedAt = DateTime.UtcNow;
            paymentMethod.UpdatedAt = DateTime.UtcNow;

            // If this is the first payment method for the user, set it as default
            var existingPaymentMethods = await _paymentMethodRepository.GetPaymentMethodsByUserIdAsync(userId);
            var existingList = existingPaymentMethods.ToList();
            
            if (existingList.Count == 0)
            {
                paymentMethod.IsDefault = true;
            }

            return await _paymentMethodRepository.CreatePaymentMethodAsync(paymentMethod);
        }

        public async Task<PaymentMethod> GetPaymentMethodByIdAsync(string paymentMethodId, Guid userId)
        {
            var paymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(paymentMethodId);
            
            // Verify that the payment method belongs to the user
            if (paymentMethod == null || paymentMethod.UserId != userId)
            {
                return null;
            }

            return paymentMethod;
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsByUserIdAsync(Guid userId)
        {
            return await _paymentMethodRepository.GetPaymentMethodsByUserIdAsync(userId);
        }

        public async Task<PaymentMethod> UpdatePaymentMethodAsync(string paymentMethodId, Guid userId, PaymentMethod paymentMethod)
        {
            var existingPaymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(paymentMethodId);
            
            // Verify that the payment method belongs to the user
            if (existingPaymentMethod == null || existingPaymentMethod.UserId != userId)
            {
                return null;
            }

            // Update the payment method properties
            existingPaymentMethod.Type = paymentMethod.Type;
            existingPaymentMethod.CardNumber = paymentMethod.CardNumber;
            existingPaymentMethod.ExpiryDate = paymentMethod.ExpiryDate;
            existingPaymentMethod.CardholderName = paymentMethod.CardholderName;
            existingPaymentMethod.UpdatedAt = DateTime.UtcNow;

            // Only update the default status if explicitly provided in the update
            if (paymentMethod.IsDefault)
            {
                // If changing this to default, set all other payment methods for this user as non-default
                await _paymentMethodRepository.SetDefaultPaymentMethodAsync(userId, paymentMethodId);
            }
            else
            {
                // If not setting as default, just update the record
                existingPaymentMethod.IsDefault = paymentMethod.IsDefault;
            }

            return await _paymentMethodRepository.UpdatePaymentMethodAsync(existingPaymentMethod);
        }

        public async Task<bool> DeletePaymentMethodAsync(string paymentMethodId, Guid userId)
        {
            var existingPaymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(paymentMethodId);
            
            // Verify that the payment method belongs to the user
            if (existingPaymentMethod == null || existingPaymentMethod.UserId != userId)
            {
                return false;
            }

            return await _paymentMethodRepository.DeletePaymentMethodAsync(paymentMethodId);
        }

        public async Task<PaymentMethod> SetDefaultPaymentMethodAsync(Guid userId, string paymentMethodId)
        {
            return await _paymentMethodRepository.SetDefaultPaymentMethodAsync(userId, paymentMethodId);
        }
    }
}