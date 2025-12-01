using B2BMarketplace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Core.Interfaces.Repositories
{
    public interface IPaymentMethodRepository
    {
        Task<PaymentMethod> CreatePaymentMethodAsync(PaymentMethod paymentMethod);
        Task<PaymentMethod> GetPaymentMethodByIdAsync(string paymentMethodId);
        Task<IEnumerable<PaymentMethod>> GetPaymentMethodsByUserIdAsync(Guid userId);
        Task<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod paymentMethod);
        Task<bool> DeletePaymentMethodAsync(string paymentMethodId);
        Task<PaymentMethod> SetDefaultPaymentMethodAsync(Guid userId, string paymentMethodId);
        Task<bool> SaveChangesAsync();
    }
}