using B2BMarketplace.Core.Interfaces.Services.Admin;
using B2BMarketplace.Core.Interfaces.Services.Premium;
using B2BMarketplace.Core.Services.Admin;
using B2BMarketplace.Core.Services.Premium;

namespace B2BMarketplace.Api.Configuration
{
    /// <summary>
    /// Configuration for Admin and Premium features
    /// </summary>
    public static class AdminFeatureConfiguration
    {
        /// <summary>
        /// Registers admin and premium related services
        /// </summary>
        /// <param name="services">Service collection to register services to</param>
        public static void RegisterAdminAndPremiumServices(this IServiceCollection services)
        {
            // Admin services
            services.AddScoped<IAdminCategoryService, AdminCategoryService>();
            services.AddScoped<IAdminCertificationService, AdminCertificationService>();

            // Premium services
            services.AddScoped<IPremiumAssignmentService, PremiumAssignmentService>();
            services.AddScoped<IPremiumManagementService, PremiumManagementService>();
        }
    }
}