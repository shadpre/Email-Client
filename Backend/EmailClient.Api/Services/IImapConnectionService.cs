using MailKit;
using MailKit.Net.Imap;
using EmailClient.Api.Models;

namespace EmailClient.Api.Services
{
    /// <summary>
    /// Handles IMAP server connection management and authentication.
    /// Implements the Single Responsibility Principle by focusing only on connection concerns.
    /// </summary>
    public interface IImapConnectionService
    {
        /// <summary>
        /// Establishes a connection to an IMAP server using the provided configuration
        /// </summary>
        /// <param name="config">IMAP server configuration including credentials</param>
        /// <returns>True if connection and authentication successful, false otherwise</returns>
        Task<bool> ConnectAsync(ImapConfig config);

        /// <summary>
        /// Closes the IMAP connection and releases resources
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets the current IMAP client instance (null if not connected)
        /// </summary>
        ImapClient? Client { get; }

        /// <summary>
        /// Gets the inbox folder reference (null if not connected)
        /// </summary>
        IMailFolder? Inbox { get; }

        /// <summary>
        /// Indicates whether the service is currently connected to an IMAP server
        /// </summary>
        bool IsConnected { get; }
    }
}
