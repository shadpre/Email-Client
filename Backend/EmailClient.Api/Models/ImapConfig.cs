namespace EmailClient.Api.Models
{
    /// <summary>
    /// Configuration settings required to establish an IMAP connection to an email server.
    /// Contains server details, authentication credentials, and connection preferences.
    /// </summary>
    public class ImapConfig
    {
        /// <summary>
        /// The IMAP server hostname (e.g., "imap.gmail.com", "outlook.office365.com")
        /// </summary>
        public string Server { get; set; } = string.Empty;

        /// <summary>
        /// The port number for the IMAP connection (typically 993 for SSL, 143 for non-SSL)
        /// </summary>
        public int Port { get; set; } = 993;

        /// <summary>
        /// The username/email address for authentication
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The password for authentication (use App Password for providers like Gmail)
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use SSL/TLS encryption for the connection (recommended: true)
        /// </summary>
        public bool UseSsl { get; set; } = true;
    }
}
