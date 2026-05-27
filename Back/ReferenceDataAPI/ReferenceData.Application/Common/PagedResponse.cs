using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Application.Common
{
    /// <summary>
    /// Wrapper for responses that return a paginated list of items.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list.</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        /// The collection of items for the current page.
        /// </summary>
        public List<T> Data { get; set; } = [];

        /// <summary>
        /// The current page number (1-indexed).
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// The number of items requested per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total count of records available in the database for the given query.
        /// </summary>
        public int Total { get; set; }
    }
}
