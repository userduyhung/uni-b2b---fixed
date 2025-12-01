namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for locking a user account
    /// </summary>
    public class LockUserRequestDto
    {
        /// <summary>
        /// Reason for locking the user account
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
}