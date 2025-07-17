using MailKit;
using MailKit.Search;
using EmailClient.Api.Models;

namespace EmailClient.Api.Services
{
    /// <summary>
    /// Implementation of email deletion operations for IMAP servers.
    /// Ensures emails are permanently removed from the server through proper expunge operations.
    /// </summary>
    public class EmailDeletionService : IEmailDeletionService
    {
        private readonly IImapConnectionService _connectionService;

        public EmailDeletionService(IImapConnectionService connectionService)
        {
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        }

        /// <inheritdoc />
        public async Task<int> DeleteEmailsAsync(List<uint> uids)
        {
            ValidateConnection();

            if (uids == null || uids.Count == 0)
            {
                return 0;
            }

            try
            {
                var inbox = _connectionService.Inbox!;
                
                // Convert uint UIDs to UniqueId objects
                var uniqueIds = uids.Select(uid => new UniqueId(uid)).ToList();
                
                
                // Mark emails as deleted
                await inbox.AddFlagsAsync(uniqueIds, MessageFlags.Deleted, true);
                
                // Permanently remove marked emails from server
                await inbox.ExpungeAsync();
                
                return uids.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete emails: {ex.Message}", ex);
            }
        }

        /// <inheritdoc />
        public async Task<int> DeleteEmailsBySenderAsync(string senderEmail)
        {
            ValidateConnection();

            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new ArgumentException("Sender email address cannot be null or empty", nameof(senderEmail));
            }

            try
            {
                var inbox = _connectionService.Inbox!;
                
                
                // Search for emails from the specific sender
                var query = SearchQuery.FromContains(senderEmail);
                var uids = await inbox.SearchAsync(query);
                
                if (uids.Count == 0)
                {
                    return 0;
                }


                // Mark emails as deleted
                await inbox.AddFlagsAsync(uids, MessageFlags.Deleted, true);
                
                // Permanently remove marked emails from server
                await inbox.ExpungeAsync();
                
                return uids.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete emails from sender: {ex.Message}", ex);
            }
        }

        /// <inheritdoc />
        public async Task<int> DeleteEmailsBySenderWithFilterAsync(string senderEmail, DateFilter? dateFilter)
        {
            ValidateConnection();

            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new ArgumentException("Sender email address cannot be null or empty", nameof(senderEmail));
            }

            try
            {
                var inbox = _connectionService.Inbox!;
                
                
                // Build combined search query for sender and date filter
                var senderQuery = SearchQuery.FromContains(senderEmail);
                var dateQuery = BuildSearchQuery(dateFilter);
                
                SearchQuery combinedQuery;
                if (dateQuery == SearchQuery.All)
                {
                    combinedQuery = senderQuery;
                }
                else
                {
                    combinedQuery = SearchQuery.And(senderQuery, dateQuery);
                }
                
                var uids = await inbox.SearchAsync(combinedQuery);
                
                if (uids.Count == 0)
                {
                    return 0;
                }


                // Mark emails as deleted
                await inbox.AddFlagsAsync(uids, MessageFlags.Deleted, true);
                
                // Permanently remove marked emails from server
                await inbox.ExpungeAsync();
                
                return uids.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete filtered emails from sender: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Builds a search query based on the provided date filter
        /// </summary>
        /// <param name="dateFilter">Date filter configuration</param>
        /// <returns>SearchQuery for the date filter</returns>
        private static SearchQuery BuildSearchQuery(DateFilter? dateFilter)
        {
            if (dateFilter == null || dateFilter.FilterType == DateFilterType.All)
            {
                return SearchQuery.All;
            }

            switch (dateFilter.FilterType)
            {
                case DateFilterType.OlderThanDays:
                    if (dateFilter.Days.HasValue)
                    {
                        var cutoffDate = DateTime.Now.AddDays(-dateFilter.Days.Value);
                        return SearchQuery.DeliveredBefore(cutoffDate);
                    }
                    break;

                case DateFilterType.OlderThanMonths:
                    if (dateFilter.Months.HasValue)
                    {
                        var cutoffDate = DateTime.Now.AddMonths(-dateFilter.Months.Value);
                        return SearchQuery.DeliveredBefore(cutoffDate);
                    }
                    break;

                case DateFilterType.OlderThanYears:
                    if (dateFilter.Years.HasValue)
                    {
                        var cutoffDate = DateTime.Now.AddYears(-dateFilter.Years.Value);
                        return SearchQuery.DeliveredBefore(cutoffDate);
                    }
                    break;

                case DateFilterType.DateRange:
                    if (dateFilter.StartDate.HasValue && dateFilter.EndDate.HasValue)
                    {
                        return SearchQuery.And(
                            SearchQuery.DeliveredAfter(dateFilter.StartDate.Value),
                            SearchQuery.DeliveredBefore(dateFilter.EndDate.Value.AddDays(1))
                        );
                    }
                    else if (dateFilter.StartDate.HasValue)
                    {
                        return SearchQuery.DeliveredAfter(dateFilter.StartDate.Value);
                    }
                    else if (dateFilter.EndDate.HasValue)
                    {
                        return SearchQuery.DeliveredBefore(dateFilter.EndDate.Value.AddDays(1));
                    }
                    break;
            }

            // Fallback to all emails if filter configuration is invalid
            return SearchQuery.All;
        }

        /// <summary>
        /// Validates that the IMAP connection is active and ready for operations
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if not connected to IMAP server</exception>
        private void ValidateConnection()
        {
            if (!_connectionService.IsConnected || _connectionService.Inbox == null)
            {
                throw new InvalidOperationException("Not connected to IMAP server");
            }
        }
    }
}
