namespace B2BMarketplace.Core.Enums
{
    /// <summary>
    /// Represents the actions that can be taken during content moderation
    /// </summary>
    public enum ModerationAction
    {
        /// <summary>
        /// Hide the content from public view
        /// </summary>
        Hide,

        /// <summary>
        /// Remove the content permanently
        /// </summary>
        Remove,

        /// <summary>
        /// Dismiss the report without taking action
        /// </summary>
        Dismiss
    }
}