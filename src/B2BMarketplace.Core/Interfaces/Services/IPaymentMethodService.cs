using B2BMarketplace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Services
{
    public interface IPaymentMethodService
    {
        Task<PaymentMethod> CreatePaymentMethodAsync(Guid userId, PaymentMethod paymentMethod);
        Task<PaymentMethod> GetPaymentMethodByIdAsync(string paymentMethodId, Guid userId);
        Task<IEnumerable<PaymentMethod>> GetPaymentMethodsByUserIdAsync(Guid userId);
        Task<PaymentMethod> UpdatePaymentMethodAsync(string paymentMethodId, Guid userId, PaymentMethod paymentMethod);
        Task<bool> DeletePaymentMethodAsync(string paymentMethodId, Guid userId);
        Task<PaymentMethod> SetDefaultPaymentMethodAsync(Guid userId, string paymentMethodId);
    }
}