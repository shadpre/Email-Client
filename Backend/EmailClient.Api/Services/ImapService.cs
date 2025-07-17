using EmailClient.Api.Models;

namespace EmailClient.Api.Services
{
    /// <summary>
    /// Main IMAP service implementation that coordinates email operations.
    /// Acts as a facade to provide a unified interface while delegating to specialized services.
    /// This design follows the Single Responsibility and Open/Closed principles.
    /// </summary>
    public class ImapService : IImapService
    {
        private readonly IImapConnectionService _connectionService;
        private readonly IEmailRetrievalService _retrievalService;
        private readonly IEmailDeletionService _deletionService;

        /// <summary>
        /// Initializes the IMAP service with its dependencies
        /// </summary>
        /// <param name="connectionService">Service for managing IMAP connections</param>
        /// <param name="retrievalService">Service for retrieving and organizing emails</param>
        /// <param name="deletionService">Service for deleting emails</param>
        public ImapService(
            IImapConnectionService connectionService,
            IEmailRetrievalService retrievalService,
            IEmailDeletionService deletionService)
        {
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
            _retrievalService = retrievalService ?? throw new ArgumentNullException(nameof(retrievalService));
            _deletionService = deletionService ?? throw new ArgumentNullException(nameof(deletionService));
        }

        /// <inheritdoc />
        public async Task<bool> ConnectAsync(ImapConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            return await _connectionService.ConnectAsync(config);
        }

        /// <inheritdoc />
        public async Task<List<SenderGroup>> GetEmailsBySenderAsync()
        {
            return await _retrievalService.GetEmailsBySenderAsync();
        }

        /// <inheritdoc />
        public async Task<List<SenderGroup>> GetEmailsBySenderAsync(DateFilter? dateFilter)
        {
            return await _retrievalService.GetEmailsBySenderAsync(dateFilter);
        }

        /// <inheritdoc />
        public async Task<int> DeleteEmailsAsync(List<uint> uids)
        {
            return await _deletionService.DeleteEmailsAsync(uids);
        }

        /// <inheritdoc />
        public async Task<int> DeleteEmailsBySenderAsync(string senderEmail)
        {
            return await _deletionService.DeleteEmailsBySenderAsync(senderEmail);
        }

        /// <inheritdoc />
        public async Task<int> DeleteEmailsBySenderWithFilterAsync(string senderEmail, DateFilter? dateFilter)
        {
            return await _deletionService.DeleteEmailsBySenderWithFilterAsync(senderEmail, dateFilter);
        }

        /// <inheritdoc />
        public ProcessingStatus GetProcessingStatus()
        {
            return _retrievalService.GetProcessingStatus();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            _connectionService.Disconnect();
        }
    }
}
