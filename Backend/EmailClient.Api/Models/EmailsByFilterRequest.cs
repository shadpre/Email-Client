namespace EmailClient.Api.Models
{
    /// <summary>
    /// Request model for retrieving emails grouped by sender with optional date filtering.
    /// Allows filtering emails by date ranges for more targeted mailbox cleanup.
    /// </summary>
    public class EmailsByFilterRequest
    {
        /// <summary>
        /// Optional date filter to apply when retrieving emails.
        /// If null, all emails will be retrieved.
        /// </summary>
        public DateFilter? DateFilter { get; set; }

        /// <summary>
        /// Creates a request with no date filtering (all emails)
        /// </summary>
        /// <returns>EmailsByFilterRequest configured for all emails</returns>
        public static EmailsByFilterRequest All()
        {
            return new EmailsByFilterRequest();
        }

        /// <summary>
        /// Creates a request filtered by date range
        /// </summary>
        /// <param name="dateFilter">Date filter configuration</param>
        /// <returns>EmailsByFilterRequest configured with date filter</returns>
        public static EmailsByFilterRequest WithDateFilter(DateFilter dateFilter)
        {
            return new EmailsByFilterRequest
            {
                DateFilter = dateFilter
            };
        }
    }
}
