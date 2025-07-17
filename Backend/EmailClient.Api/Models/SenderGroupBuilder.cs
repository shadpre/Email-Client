namespace EmailClient.Api.Models
{
    /// <summary>
    /// Internal helper class used during the email grouping process.
    /// Efficiently accumulates email statistics and samples while processing large mailboxes in batches.
    /// </summary>
    internal class SenderGroupBuilder
    {
        /// <summary>
        /// The email address of the sender being processed
        /// </summary>
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>
        /// The display name of the sender being processed
        /// </summary>
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// Running count of emails from this sender (incremented during batch processing)
        /// </summary>
        public int EmailCount { get; set; }

        /// <summary>
        /// Running total of bytes used by emails from this sender
        /// </summary>
        public long TotalSize { get; set; }

        /// <summary>
        /// Collection of email samples for preview (limited to prevent memory issues)
        /// </summary>
        public List<EmailSummary> Emails { get; set; } = new();
    }
}
