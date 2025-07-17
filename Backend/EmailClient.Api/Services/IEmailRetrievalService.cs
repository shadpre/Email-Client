using EmailClient.Api.Models;

namespace EmailClient.Api.Services
{
    /// <summary>
    /// Service for retrieving and organizing emails from an IMAP server.
    /// Focuses on email retrieval and grouping operations while delegating connection management.
    /// </summary>
    public interface IEmailRetrievalService
    {
        /// <summary>
        /// Retrieves all emails from the inbox and groups them by sender.
        /// Processes emails in batches for memory efficiency with large mailboxes.
        /// </summary>
        /// <returns>Collection of sender groups with email counts and samples</returns>
        Task<List<SenderGroup>> GetEmailsBySenderAsync();

        /// <summary>
        /// Retrieves emails from the inbox grouped by sender with optional date filtering.
        /// Processes emails in batches for memory efficiency with large mailboxes.
        /// </summary>
        /// <param name="dateFilter">Optional date filter to apply when retrieving emails</param>
        /// <returns>Collection of sender groups with email counts and samples</returns>
        Task<List<SenderGroup>> GetEmailsBySenderAsync(DateFilter? dateFilter);

        /// <summary>
        /// Gets the current processing status for long-running operations
        /// </summary>
        /// <returns>Current processing status with progress information</returns>
        ProcessingStatus GetProcessingStatus();
    }
}
