using MailKit;
using MailKit.Search;

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
                Console.WriteLine("No email UIDs provided for deletion");
                return 0;
            }

            try
            {
                var inbox = _connectionService.Inbox!;
                
                // Convert uint UIDs to UniqueId objects
                var uniqueIds = uids.Select(uid => new UniqueId(uid)).ToList();
                
                Console.WriteLine($"Marking {uniqueIds.Count} emails for deletion");
                
                // Mark emails as deleted
                await inbox.AddFlagsAsync(uniqueIds, MessageFlags.Deleted, true);
                
                // Permanently remove marked emails from server
                await inbox.ExpungeAsync();
                
                Console.WriteLine($"Successfully deleted {uids.Count} emails");
                return uids.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete emails: {ex.Message}");
                throw new InvalidOperationException($"Failed to delete emails: {ex.Message}", ex);
            }
        }

        /// <inheritdoc />
        public async Task<int> DeleteEmailsBySenderAsync(string senderEmail)
        {
            ValidateConnection();

            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                Console.WriteLine("Sender email address is required for deletion");
                throw new ArgumentException("Sender email address cannot be null or empty", nameof(senderEmail));
            }

            try
            {
                var inbox = _connectionService.Inbox!;
                
                Console.WriteLine($"Searching for emails from sender: {senderEmail}");
                
                // Search for emails from the specific sender
                var query = SearchQuery.FromContains(senderEmail);
                var uids = await inbox.SearchAsync(query);
                
                if (uids.Count == 0)
                {
                    Console.WriteLine($"No emails found from sender: {senderEmail}");
                    return 0;
                }

                Console.WriteLine($"Found {uids.Count} emails from {senderEmail}, marking for deletion");

                // Mark emails as deleted
                await inbox.AddFlagsAsync(uids, MessageFlags.Deleted, true);
                
                // Permanently remove marked emails from server
                await inbox.ExpungeAsync();
                
                Console.WriteLine($"Successfully deleted {uids.Count} emails from {senderEmail}");
                return uids.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete emails from {senderEmail}: {ex.Message}");
                throw new InvalidOperationException($"Failed to delete emails from sender: {ex.Message}", ex);
            }
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
