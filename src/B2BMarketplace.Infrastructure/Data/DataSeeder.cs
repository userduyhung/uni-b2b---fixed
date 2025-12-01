using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace B2BMarketplace.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var changesMade = false;
            
            // Create admin user if it doesn't exist
            var adminExists = await context.Users.AnyAsync(u => u.Email == "admin@b2bmarketplace.com");
            if (!adminExists)
            {
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@b2bmarketplace.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPass123!"),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsLocked = false
                };

                context.Users.Add(adminUser);
                changesMade = true;
            }

            // Check if we already have data
            if (await context.RFQs.AnyAsync())
            {
                // Still save changes if we made any (like creating admin user)
                if (changesMade)
                {
                    await context.SaveChangesAsync();
                }
                return; // Data already seeded
            }

            // Rest of the seeding code remains the same...

            // Create a sample buyer user and profile
            var buyerUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "buyer@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = UserRole.Buyer,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsLocked = false
            };

            context.Users.Add(buyerUser);

            var buyerProfile = new BuyerProfile
            {
                Id = Guid.NewGuid(),
                UserId = buyerUser.Id,
                Name = "Sample Buyer",
                CompanyName = "Manufacturing Corp",
                Country = "USA",
                Phone = "+1234567890",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.BuyerProfiles.Add(buyerProfile);

            // Create sample RFQs
            var rfq1 = new RFQ
            {
                Id = Guid.NewGuid(),
                Title = "Industrial Equipment Request",
                Description = "Looking for high-quality industrial manufacturing equipment for our production line. Requirements include durability, efficiency, and compliance with safety standards.",
                BuyerProfileId = buyerProfile.Id,
                Status = RFQStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            var rfq2 = new RFQ
            {
                Id = Guid.NewGuid(),
                Title = "Technology Solutions Package",
                Description = "Seeking comprehensive technology solutions including software, hardware, and support services for digital transformation initiative.",
                BuyerProfileId = buyerProfile.Id,
                Status = RFQStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            context.RFQs.AddRange(rfq1, rfq2);

            // Add RFQ Items
            var rfqItem1 = new RFQItem
            {
                Id = Guid.NewGuid(),
                RFQId = rfq1.Id,
                ProductName = "Industrial Press Machine",
                Description = "Heavy-duty press machine for metal forming operations",
                Quantity = 2,
                Unit = "units"
            };

            var rfqItem2 = new RFQItem
            {
                Id = Guid.NewGuid(),
                RFQId = rfq2.Id,
                ProductName = "ERP Software License",
                Description = "Enterprise resource planning software with manufacturing modules",
                Quantity = 1,
                Unit = "license"
            };

            context.RFQItems.AddRange(rfqItem1, rfqItem2);

            await context.SaveChangesAsync();
        }
    }
}