using MailKit;
using MailKit.Net.Imap;
using EmailClient.Api.Models;

namespace EmailClient.Api.Services
{
    /// <summary>
    /// Concrete implementation of IMAP connection management.
    /// Handles the technical details of connecting to and authenticating with IMAP servers.
    /// </summary>
    public class ImapConnectionService : IImapConnectionService
    {
        private ImapClient? _client;
        private IMailFolder? _inbox;

        /// <inheritdoc />
        public ImapClient? Client => _client;

        /// <inheritdoc />
        public IMailFolder? Inbox => _inbox;

        /// <inheritdoc />
        public bool IsConnected => _client?.IsConnected == true;

        /// <inheritdoc />
        public async Task<bool> ConnectAsync(ImapConfig config)
        {
            try
            {
                Console.WriteLine($"Attempting to connect to {config.Server}:{config.Port} with SSL: {config.UseSsl}");
                
                // Create new IMAP client instance
                _client = new ImapClient();
                
                // Establish connection to the server
                await _client.ConnectAsync(config.Server, config.Port, config.UseSsl);
                Console.WriteLine("Connected to server, attempting authentication...");
                
                // Authenticate with provided credentials
                await _client.AuthenticateAsync(config.Username, config.Password);
                Console.WriteLine("Authentication successful, opening inbox...");
                
                // Open the inbox folder with read/write access
                _inbox = _client.Inbox;
                await _inbox.OpenAsync(FolderAccess.ReadWrite);
                Console.WriteLine("Inbox opened successfully");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                
                // Clean up on failure
                _client?.Dispose();
                _client = null;
                _inbox = null;
                
                return false;
            }
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            try
            {
                // Gracefully disconnect and clean up resources
                _client?.Disconnect(true);
                _client?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during disconnect: {ex.Message}");
            }
            finally
            {
                _client = null;
                _inbox = null;
            }
        }
    }
}
