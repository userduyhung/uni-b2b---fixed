using System.ComponentModel.DataAnnotations;
using B2BMarketplace.Core.DTOs;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for seller profile with certifications
    /// </summary>
    public class SellerProfileWithCertificationsDto : SellerProfileDto
    {
        /// <summary>
        /// List of certifications for the seller
        /// </summary>
        public List<CertificationDto> Certifications { get; set; } = new();
    }
}