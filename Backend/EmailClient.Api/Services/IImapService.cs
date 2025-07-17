using EmailClient.Api.Models;

namespace EmailClient.Api.Services
{
    /// <summary>
    /// Main facade service that coordinates IMAP email operations.
    /// Implements the Facade pattern to provide a simplified interface for email management.
    /// Delegates specific operations to specialized services following Single Responsibility Principle.
    /// </summary>
    public interface IImapService
    {
        /// <summary>
        /// Establishes a connection to an IMAP server
        /// </summary>
        /// <param name="config">IMAP server configuration</param>
        /// <returns>True if connection successful, false otherwise</returns>
        Task<bool> ConnectAsync(ImapConfig config);

        /// <summary>
        /// Retrieves all emails grouped by sender
        /// </summary>
        /// <returns>Collection of sender groups with email statistics</returns>
        Task<List<SenderGroup>> GetEmailsBySenderAsync();

        /// <summary>
        /// Retrieves emails grouped by sender with optional date filtering
        /// </summary>
        /// <param name="dateFilter">Optional date filter to apply when retrieving emails</param>
        /// <returns>Collection of sender groups with email statistics</returns>
        Task<List<SenderGroup>> GetEmailsBySenderAsync(DateFilter? dateFilter);

        /// <summary>
        /// Deletes specific emails by their UIDs
        /// </summary>
        /// <param name="uids">Email unique identifiers to delete</param>
        /// <returns>Number of emails deleted</returns>
        Task<int> DeleteEmailsAsync(List<uint> uids);

        /// <summary>
        /// Deletes all emails from a specific sender
        /// </summary>
        /// <param name="senderEmail">Sender's email address</param>
        /// <returns>Number of emails deleted</returns>
        Task<int> DeleteEmailsBySenderAsync(string senderEmail);

        /// <summary>
        /// Deletes emails from a specific sender that match the date filter criteria
        /// </summary>
        /// <param name="senderEmail">Sender's email address</param>
        /// <param name="dateFilter">Optional date filter to apply when deleting emails</param>
        /// <returns>Number of emails deleted</returns>
        Task<int> DeleteEmailsBySenderWithFilterAsync(string senderEmail, DateFilter? dateFilter);

        /// <summary>
        /// Gets the current processing status for long-running operations
        /// </summary>
        /// <returns>Processing status with progress information</returns>
        ProcessingStatus GetProcessingStatus();

        /// <summary>
        /// Disconnects from the IMAP server and releases resources
        /// </summary>
        void Disconnect();
    }
}
