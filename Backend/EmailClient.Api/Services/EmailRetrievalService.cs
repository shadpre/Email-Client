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
            Console.WriteLine($"GetEmailsBySenderAsync called. Client connected: {_connectionService.IsConnected}");
            
            // Validate connection state
            if (!_connectionService.IsConnected || _connectionService.Inbox == null)
            {
                Console.WriteLine("Not connected to IMAP server");
                throw new InvalidOperationException("Not connected to IMAP server");
            }

            try
            {
                return await ProcessEmailsInBatches();
            }
            catch (Exception ex)
            {
                // Reset processing status on error
                _processingStatus.IsProcessing = false;
                _processingStatus.CurrentOperation = "Error occurred";
                Console.WriteLine($"Error in GetEmailsBySenderAsync: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve emails: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Processes all emails in the inbox using batch processing for memory efficiency
        /// </summary>
        /// <returns>List of sender groups with aggregated email information</returns>
        private async Task<List<SenderGroup>> ProcessEmailsInBatches()
        {
            var inbox = _connectionService.Inbox!;
            
            // Ensure inbox is open for reading
            if (!inbox.IsOpen)
                await inbox.OpenAsync(FolderAccess.ReadWrite);

            // Get total email count
            var totalCount = inbox.Count;
            Console.WriteLine($"Total emails in inbox: {totalCount}");

            if (totalCount == 0)
            {
                Console.WriteLine("No emails found in inbox");
                return new List<SenderGroup>();
            }

            // Search for all emails (gets UIDs without downloading content)
            var uids = await inbox.SearchAsync(SearchQuery.All);
            Console.WriteLine($"Found {uids.Count} email UIDs");

            if (uids.Count == 0)
            {
                Console.WriteLine("No email UIDs returned from search");
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
            
            Console.WriteLine($"Processing {totalEmails} emails in batches of {BatchSize}...");
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
                
                Console.WriteLine($"Processing batch {_processingStatus.CurrentBatch}/{_processingStatus.TotalBatches} ({batch.Count} emails)");
                
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
            var fromAddress = message.Envelope!.From.FirstOrDefault()?.ToString() ?? "Unknown";
            var senderEmail = _emailParsingService.ExtractEmail(fromAddress);
            
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
            return senderEmailCounts.Values
                .Select(builder => new SenderGroup
                {
                    SenderEmail = builder.SenderEmail,
                    SenderName = builder.SenderName,
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
            Console.WriteLine($"Created {_processingStatus.ProcessedEmails} email entries across multiple senders");
        }
    }
}
