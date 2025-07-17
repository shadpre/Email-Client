namespace EmailClient.Api.Models
{
    /// <summary>
    /// Represents a collection of emails grouped by sender.
    /// Provides aggregated statistics and a sample of recent emails for efficient mailbox management.
    /// </summary>
    public class SenderGroup
    {
        /// <summary>
        /// The email address of the sender (used as the primary identifier)
        /// </summary>
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>
        /// The display name of the sender (fallback to email if name not available)
        /// </summary>
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// Total number of emails from this sender in the mailbox
        /// </summary>
        public int EmailCount { get; set; }

        /// <summary>
        /// Combined size of all emails from this sender in bytes
        /// </summary>
        public long TotalSize { get; set; }

        /// <summary>
        /// A limited collection of recent emails from this sender for preview purposes.
        /// Limited to 10 emails for performance optimization.
        /// </summary>
        public List<EmailSummary> Emails { get; set; } = new();
    }
}
