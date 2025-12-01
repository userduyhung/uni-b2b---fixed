using System.ComponentModel.DataAnnotations;

namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// DTO for approving a verification request
    /// </summary>
    public class ApproveVerificationRequest
    {
        /// <summary>
        /// Comments for the approval
        /// </summary>
        [StringLength(500)]
        public string Comments { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for rejecting a verification request
    /// </summary>
    public class RejectVerificationRequest
    {
        /// <summary>
        /// Reason for rejection
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}