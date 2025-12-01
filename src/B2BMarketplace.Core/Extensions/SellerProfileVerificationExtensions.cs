using B2BMarketplace.Core.Entities;
using B2BMarketplace.Core.Enums;
using System.Collections.Generic;
using System.Linq;

namespace B2BMarketplace.Core.Extensions
{
    /// <summary>
    /// Extension methods for SellerProfile verification logic
    /// </summary>
    public static class SellerProfileVerificationExtensions
    {
        /// <summary>
        /// Determines if a seller should be verified based on certifications and premium status
        /// A seller is verified if they have at least one approved certification or an active premium subscription
        /// </summary>
        /// <param name="sellerProfile">The seller profile to check</param>
        /// <param name="certifications">List of certifications for the seller</param>
        /// <param name="hasActivePremiumSubscription">Whether the seller has an active premium subscription</param>
        /// <returns>True if the seller should be verified, false otherwise</returns>
        public static bool ShouldBeVerified(this SellerProfile sellerProfile, List<Certification> certifications, bool hasActivePremiumSubscription)
        {
            // Handle null certifications list
            if (certifications == null)
            {
                certifications = new List<Certification>();
            }

            // Check if seller has at least one approved certification
            var hasApprovedCertification = certifications.Any(c => c.Status == CertificationStatus.Approved);

            // Seller is verified if they have approved certifications OR active premium subscription
            return hasApprovedCertification || hasActivePremiumSubscription;
        }

        /// <summary>
        /// Determines if a seller should be verified based only on certifications
        /// </summary>
        /// <param name="sellerProfile">The seller profile to check</param>
        /// <param name="certifications">List of certifications for the seller</param>
        /// <returns>True if the seller should be verified based only on certifications, false otherwise</returns>
        public static bool ShouldBeVerifiedBasedOnCertifications(this SellerProfile sellerProfile, List<Certification> certifications)
        {
            // Check if seller has at least one approved certification
            return certifications.Any(c => c.Status == CertificationStatus.Approved);
        }
    }
}