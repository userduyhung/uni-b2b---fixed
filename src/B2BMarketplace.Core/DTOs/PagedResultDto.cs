namespace B2BMarketplace.Core.DTOs
{
    /// <summary>
    /// Data transfer object for paged results
    /// </summary>
    /// <typeparam name="T">The type of items in the result</typeparam>
    public class PagedResultDto<T>
    {
        /// <summary>
        /// The items in the current page
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items available (across all pages)
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indicates whether there is a previous page
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// Indicates whether there is a next page
        /// </summary>
        public bool HasNextPage { get; set; }

        // Alternative property names for compatibility with existing code
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalCount { get; set; }
    }
}