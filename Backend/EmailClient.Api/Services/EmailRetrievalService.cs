using MailKit;
using MailKit.Search;
using EmailClient.Api.Models;

namespace EmailClient.Api.Services
{
    /// <summary>
    /// Implementation of email retrieval and organization logic.
    /// Optimized for processing large mailboxes efficiently through batch processing.
    /// </summary>
    public class EmailRetrievalService : IEmailRetrievalService
    {
        private readonly IImapConnectionService _connectionService;
        private readonly IEmailParsingService _emailParsingService;
        private readonly ProcessingStatus _processingStatus = new();

        /// <summary>
        /// Batch size for processing emails to balance memory usage and performance
        /// </summary>
        private const int BatchSize = 2000;

        /// <summary>
        /// Maximum number of email samples to keep per sender for preview
        /// </summary>
        private const int MaxEmailSamplesPerSender = 10;

        public EmailRetrievalService(
            IImapConnectionService connectionService,
            IEmailParsingService emailParsingService)
        {
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
            _emailParsingService = emailParsingService ?? throw new ArgumentNullException(nameof(emailParsingService));
        }

        /// <inheritdoc />
        public ProcessingStatus GetProcessingStatus() => _processingStatus;

        /// <inheritdoc />
        public async Task<List<SenderGroup>> GetEmailsBySenderAsync()
        {
            return await GetEmailsBySenderAsync(null);
        }

        /// <inheritdoc />
        public async Task<List<SenderGroup>> GetEmailsBySenderAsync(DateFilter? dateFilter)
        {
            // Validate connection state
            if (!_connectionService.IsConnected || _connectionService.Inbox == null)
            {
                throw new InvalidOperationException("Not connected to IMAP server");
            }

            try
            {
                return await ProcessEmailsInBatches(dateFilter);
            }
            catch (Exception ex)
            {
                // Reset processing status on error
                _processingStatus.IsProcessing = false;
                _processingStatus.CurrentOperation = "Error occurred";
                throw new InvalidOperationException($"Failed to retrieve emails: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Processes all emails in the inbox using batch processing for memory efficiency
        /// </summary>
        /// <param name="dateFilter">Optional date filter to apply</param>
        /// <returns>List of sender groups with aggregated email information</returns>
        private async Task<List<SenderGroup>> ProcessEmailsInBatches(DateFilter? dateFilter = null)
        {
            var inbox = _connectionService.Inbox!;
            
            // Ensure inbox is open for reading
            if (!inbox.IsOpen)
                await inbox.OpenAsync(FolderAccess.ReadWrite);

            // Get total email count
            var totalCount = inbox.Count;

            if (totalCount == 0)
            {
                return new List<SenderGroup>();
            }

            // Build search query based on date filter
            var searchQuery = BuildSearchQuery(dateFilter);

            // Search for emails matching the criteria (gets UIDs without downloading content)
            var uids = await inbox.SearchAsync(searchQuery);

            if (uids.Count == 0)
            {
                return new List<SenderGroup>();
            }

            // Initialize processing status
            InitializeProcessingStatus(uids.Count);

            // Process emails in batches to manage memory
            var senderGroups = await ProcessEmailBatches(inbox, uids);

            // Mark processing as complete
            CompleteProcessing();

            return senderGroups;
        }

        /// <summary>
        /// Initializes the processing status for tracking progress
        /// </summary>
        /// <param name="totalEmails">Total number of emails to process</param>
        private void InitializeProcessingStatus(int totalEmails)
        {
            _processingStatus.IsProcessing = true;
            _processingStatus.TotalEmails = totalEmails;
            _processingStatus.ProcessedEmails = 0;
            _processingStatus.TotalBatches = (totalEmails + BatchSize - 1) / BatchSize;
            _processingStatus.CurrentOperation = "Processing emails";
        }

        /// <summary>
        /// Processes emails in batches and groups them by sender
        /// </summary>
        /// <param name="inbox">The inbox folder to process</param>
        /// <param name="uids">List of email UIDs to process</param>
        /// <returns>List of sender groups</returns>
        private async Task<List<SenderGroup>> ProcessEmailBatches(IMailFolder inbox, IList<UniqueId> uids)
        {
            var senderEmailCounts = new Dictionary<string, SenderGroupBuilder>();

            // Process emails in batches
            for (int i = 0; i < uids.Count; i += BatchSize)
            {
                var batch = uids.Skip(i).Take(BatchSize).ToList();
                _processingStatus.CurrentBatch = i / BatchSize + 1;
                
                
                await ProcessBatch(inbox, batch, senderEmailCounts);
            }

            // Convert builder objects to final result
            return ConvertToSenderGroups(senderEmailCounts);
        }

        /// <summary>
        /// Processes a single batch of emails
        /// </summary>
        /// <param name="inbox">The inbox folder</param>
        /// <param name="batch">UIDs of emails in this batch</param>
        /// <param name="senderEmailCounts">Dictionary to accumulate sender statistics</param>
        private async Task ProcessBatch(
            IMailFolder inbox, 
            List<UniqueId> batch, 
            Dictionary<string, SenderGroupBuilder> senderEmailCounts)
        {
            // Fetch only envelope data for efficiency (no body content)
            var messages = await inbox.FetchAsync(batch, 
                MessageSummaryItems.Envelope | MessageSummaryItems.Size | MessageSummaryItems.UniqueId);
            
            // Process each message in the batch
            foreach (var message in messages.Where(m => m.Envelope?.From != null))
            {
                ProcessSingleMessage(message, senderEmailCounts);
                _processingStatus.ProcessedEmails++;
            }
        }

        /// <summary>
        /// Processes a single email message and updates sender statistics
        /// </summary>
        /// <param name="message">The email message summary</param>
        /// <param name="senderEmailCounts">Dictionary to update with sender information</param>
        private void ProcessSingleMessage(
            IMessageSummary message, 
            Dictionary<string, SenderGroupBuilder> senderEmailCounts)
        {
            // Skip messages with missing or invalid envelope data
            if (message.Envelope?.From == null || !message.Envelope.From.Any())
            {
                return;
            }

            var fromAddress = message.Envelope.From.FirstOrDefault()?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                return;
            }

            var senderEmail = _emailParsingService.ExtractEmail(fromAddress);
            
            // Only skip if we really can't extract anything useful
            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                return;
            }

            // Create or get existing sender group builder
            if (!senderEmailCounts.ContainsKey(senderEmail))
            {
                senderEmailCounts[senderEmail] = new SenderGroupBuilder
                {
                    SenderEmail = senderEmail,
                    SenderName = _emailParsingService.ExtractName(fromAddress),
                    Emails = new List<EmailSummary>()
                };
            }
            
            var builder = senderEmailCounts[senderEmail];
            builder.EmailCount++;
            builder.TotalSize += message.Size ?? 0;
            
            // Only keep a limited number of email samples per sender to manage memory
            if (builder.Emails.Count < MaxEmailSamplesPerSender)
            {
                builder.Emails.Add(new EmailSummary
                {
                    Uid = message.UniqueId.Id,
                    Subject = message.Envelope.Subject ?? "No Subject",
                    From = fromAddress,
                    FromEmail = senderEmail,
                    Date = message.Envelope.Date?.DateTime ?? DateTime.MinValue,
                    Size = message.Size ?? 0
                });
            }
        }

        /// <summary>
        /// Converts builder objects to final sender groups and sorts them
        /// </summary>
        /// <param name="senderEmailCounts">Dictionary of sender group builders</param>
        /// <returns>Sorted list of sender groups</returns>
        private static List<SenderGroup> ConvertToSenderGroups(Dictionary<string, SenderGroupBuilder> senderEmailCounts)
        {
            
            var validGroups = senderEmailCounts.Values
                .Where(builder => 
                    builder.EmailCount > 0 && // Only include groups with actual emails
                    !string.IsNullOrWhiteSpace(builder.SenderEmail) && // Must have valid email
                    builder.SenderEmail != "Unknown" && // Filter out unknown senders
                    builder.SenderEmail != "unknown@example.com" && // Filter out placeholder emails
                    builder.SenderEmail.Contains("@") && // Basic email validation
                    !builder.SenderEmail.StartsWith("unknown", StringComparison.OrdinalIgnoreCase) // Filter out unknown variants
                )
                .ToList();
                
            
            // Log some examples of final data
            var examples = validGroups.Take(3);
            foreach (var example in examples)
            {
            }

            return validGroups
                .Select(builder => new SenderGroup
                {
                    SenderEmail = builder.SenderEmail,
                    SenderName = !string.IsNullOrWhiteSpace(builder.SenderName) ? builder.SenderName : builder.SenderEmail,
                    EmailCount = builder.EmailCount,
                    TotalSize = builder.TotalSize,
                    Emails = builder.Emails.OrderByDescending(e => e.Date).ToList()
                })
                .OrderByDescending(g => g.EmailCount) // Sort by email count (most emails first)
                .ToList();
        }

        /// <summary>
        /// Marks the processing operation as complete
        /// </summary>
        private void CompleteProcessing()
        {
            _processingStatus.IsProcessing = false;
            _processingStatus.CurrentOperation = "Completed";
        }

        /// <summary>
        /// Builds IMAP search query based on date filter criteria
        /// </summary>
        /// <param name="dateFilter">Date filter to apply, or null for all emails</param>
        /// <returns>SearchQuery object for IMAP search</returns>
        private static SearchQuery BuildSearchQuery(DateFilter? dateFilter)
        {
            if (dateFilter == null || dateFilter.FilterType == DateFilterType.All)
            {
                return SearchQuery.All;
            }

            switch (dateFilter.FilterType)
            {
                case DateFilterType.OlderThanDays:
                    if (dateFilter.Days.HasValue)
                    {
                        var cutoffDate = DateTime.Now.AddDays(-dateFilter.Days.Value);
                        return SearchQuery.DeliveredBefore(cutoffDate);
                    }
                    break;

                case DateFilterType.OlderThanMonths:
                    if (dateFilter.Months.HasValue)
                    {
                        var cutoffDate = DateTime.Now.AddMonths(-dateFilter.Months.Value);
                        return SearchQuery.DeliveredBefore(cutoffDate);
                    }
                    break;

                case DateFilterType.OlderThanYears:
                    if (dateFilter.Years.HasValue)
                    {
                        var cutoffDate = DateTime.Now.AddYears(-dateFilter.Years.Value);
                        return SearchQuery.DeliveredBefore(cutoffDate);
                    }
                    break;

                case DateFilterType.DateRange:
                    if (dateFilter.StartDate.HasValue && dateFilter.EndDate.HasValue)
                    {
                        // Use AND to combine date range filters
                        return SearchQuery.And(
                            SearchQuery.DeliveredAfter(dateFilter.StartDate.Value),
                            SearchQuery.DeliveredBefore(dateFilter.EndDate.Value.AddDays(1)) // Add 1 day to make end date inclusive
                        );
                    }
                    else if (dateFilter.StartDate.HasValue)
                    {
                        return SearchQuery.DeliveredAfter(dateFilter.StartDate.Value);
                    }
                    else if (dateFilter.EndDate.HasValue)
                    {
                        return SearchQuery.DeliveredBefore(dateFilter.EndDate.Value.AddDays(1)); // Add 1 day to make end date inclusive
                    }
                    break;
            }

            // Fallback to all emails if filter configuration is invalid
            return SearchQuery.All;
        }
    }
}
