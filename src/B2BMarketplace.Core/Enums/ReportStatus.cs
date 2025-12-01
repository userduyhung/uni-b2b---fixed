namespace B2BMarketplace.Core.Enums
{
    /// <summary>
    /// Represents the status of a content report
    /// </summary>
    public enum ReportStatus
    {
        /// <summary>
        /// Report has been submitted but not yet reviewed
        /// </summary>
        Pending,

        /// <summary>
        /// Report has been resolved with action taken
        /// </summary>
        Resolved,

        /// <summary>
        /// Report has been dismissed without action
        /// </summary>
        Dismissed
    }
}