namespace EmailClient.Api.Models
{
    /// <summary>
    /// Represents a summary of an individual email message.
    /// Contains essential metadata without the full email content for performance optimization.
    /// </summary>
    public class EmailSummary
    {
        /// <summary>
        /// Unique identifier for the email message on the IMAP server
        /// </summary>
        public uint Uid { get; set; }

        /// <summary>
        /// The email subject line (displays "No Subject" if empty)
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// The original sender information as received from the email header
        /// </summary>
        public string From { get; set; } = string.Empty;

        /// <summary>
        /// The extracted email address of the sender (parsed from From field)
        /// </summary>
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// The date and time when the email was sent or received
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The size of the email in bytes (used for calculating storage usage)
        /// </summary>
        public long Size { get; set; }
    }
}
