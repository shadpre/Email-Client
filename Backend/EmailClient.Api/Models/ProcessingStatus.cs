namespace EmailClient.Api.Models
{
    /// <summary>
    /// Represents the current state of email processing operations.
    /// Provides real-time feedback during lengthy operations like processing large mailboxes.
    /// </summary>
    public class ProcessingStatus
    {
        /// <summary>
        /// Indicates whether an email processing operation is currently in progress
        /// </summary>
        public bool IsProcessing { get; set; }

        /// <summary>
        /// Total number of emails to be processed in the current operation
        /// </summary>
        public int TotalEmails { get; set; }

        /// <summary>
        /// Number of emails that have been processed so far
        /// </summary>
        public int ProcessedEmails { get; set; }

        /// <summary>
        /// Current batch number being processed (for batch operations)
        /// </summary>
        public int CurrentBatch { get; set; }

        /// <summary>
        /// Total number of batches in the current operation
        /// </summary>
        public int TotalBatches { get; set; }

        /// <summary>
        /// Human-readable description of the current operation status
        /// </summary>
        public string CurrentOperation { get; set; } = string.Empty;

        /// <summary>
        /// Calculated progress percentage (0-100) based on processed vs total emails
        /// </summary>
        public double ProgressPercentage => TotalEmails > 0 ? (double)ProcessedEmails / TotalEmails * 100 : 0;
    }
}
