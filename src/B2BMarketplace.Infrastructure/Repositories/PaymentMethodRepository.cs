using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Interfaces.Repositories;
using B2BMarketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace B2BMarketplace.Infrastructure.Repositories
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentMethodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentMethod> CreatePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Add(paymentMethod);
            await _context.SaveChangesAsync();
            return paymentMethod;
        }

        public async Task<PaymentMethod> GetPaymentMethodByIdAsync(string paymentMethodId)
        {
            return await _context.PaymentMethods.FindAsync(paymentMethodId);
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsByUserIdAsync(Guid userId)
        {
            return await _context.PaymentMethods
                .Where(pm => pm.UserId == userId)
                .ToListAsync();
        }

        public async Task<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Update(paymentMethod);
            await _context.SaveChangesAsync();
            return paymentMethod;
        }

        public async Task<bool> DeletePaymentMethodAsync(string paymentMethodId)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(paymentMethodId);
            if (paymentMethod == null) return false;

            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaymentMethod> SetDefaultPaymentMethodAsync(Guid userId, string paymentMethodId)
        {
            // First, set all payment methods for this user as non-default
            var userPaymentMethods = await _context.PaymentMethods
                .Where(pm => pm.UserId == userId)
                .ToListAsync();

            foreach (var pm in userPaymentMethods)
            {
                pm.IsDefault = pm.Id == paymentMethodId;
            }

            await _context.SaveChangesAsync();
            
            // Return the payment method that was set as default
            return await _context.PaymentMethods.FindAsync(paymentMethodId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}